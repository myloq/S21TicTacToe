// ─── Состояние ────────────────────────────────────────────────
let game = null, busy = false;
let pollTimer = null;
let accessToken = null, refreshToken = null, currentUserId = null, currentLogin = null;

// DOM Элементы
const authSection = document.getElementById('authSection');
const appSection = document.getElementById('appSection');
const userInfo = document.getElementById('userInfo');
const currentUserEl = document.getElementById('currentUser');
const authError = document.getElementById('authError');
const statusEl = document.getElementById('status');
const boardEl = document.getElementById('board');
const gameIdEl = document.getElementById('gameId');
const availableGamesBlock = document.getElementById('availableGamesBlock');
const availableGamesList = document.getElementById('availableGamesList');

// ─── Авторизация и JWT ────────────────────────────────────────
function saveTokens(access, refresh) {
    accessToken = access;
    refreshToken = refresh;
    sessionStorage.setItem('accessToken', access);
    sessionStorage.setItem('refreshToken', refresh);
}

function clearSession() {
    stopPolling();
    accessToken = null; refreshToken = null; currentUserId = null; game = null;
    sessionStorage.clear();
    appSection.classList.add('hidden');
    userInfo.classList.add('hidden');
    authSection.classList.remove('hidden');
}

// Обертка над fetch для автоматического добавления токена и обновления
async function fetchApi(url, options = {}) {
    options.headers = { ...options.headers, 'Authorization': `Bearer ${accessToken}` };
    let res = await fetch(url, options);

    // Если токен протух (401), пробуем обновить его
    if (res.status === 401 && refreshToken) {
        const refreshRes = await fetch('/auth/refresh/access', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ refreshToken })
        });

        if (refreshRes.ok) {
            const data = await refreshRes.json();
            saveTokens(data.accessToken, data.refreshToken);
            options.headers['Authorization'] = `Bearer ${accessToken}`;
            res = await fetch(url, options); // Повторяем исходный запрос
        } else {
            clearSession(); // Если рефреш не удался, разлогиниваем
        }
    }
    return res;
}

// Восстановление сессии при загрузке страницы
(function restoreSession() {
    const savedToken = sessionStorage.getItem('accessToken');
    const savedRefresh = sessionStorage.getItem('refreshToken');
    const savedUserId = sessionStorage.getItem('userId');
    const savedLogin = sessionStorage.getItem('login');

    if (savedToken && savedRefresh && savedUserId) {
        accessToken = savedToken; refreshToken = savedRefresh;
        currentUserId = savedUserId; currentLogin = savedLogin;
        currentUserEl.textContent = currentLogin;
        authSection.classList.add('hidden');
        appSection.classList.remove('hidden');
        userInfo.classList.remove('hidden');
    }
})();

document.getElementById('loginBtn').addEventListener('click', login);
document.getElementById('registerBtn').addEventListener('click', register);
document.getElementById('logoutBtn').addEventListener('click', clearSession);

async function login() {
    authError.textContent = '';
    const login = document.getElementById('loginInput').value.trim();
    const password = document.getElementById('passwordInput').value;
    if (!login || !password) return;

    try {
        const res = await fetch('/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ login, password })
        });

        if (!res.ok) { authError.textContent = 'Неверный логин или пароль'; return; }

        const data = await res.json();
        saveTokens(data.accessToken, data.refreshToken);

        // Вытаскиваем userId из JWT
        const payload = JSON.parse(atob(data.accessToken.split('.')[1]));
        currentUserId = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
        currentLogin = login;

        sessionStorage.setItem('userId', currentUserId);
        sessionStorage.setItem('login', currentLogin);

        currentUserEl.textContent = currentLogin;
        authSection.classList.add('hidden');
        appSection.classList.remove('hidden');
        userInfo.classList.remove('hidden');
    } catch (e) { authError.textContent = 'Ошибка сети'; }
}

async function register() {
    authError.textContent = '';

    // Переименовали локальные переменные, чтобы не перекрывать функцию login()
    const username = document.getElementById('loginInput').value.trim();
    const password = document.getElementById('passwordInput').value;

    if (!username || !password) return;

    try {
        const res = await fetch('/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ login: username, password }) // Отправляем username
        });

        if (res.status === 409) { authError.textContent = 'Логин занят'; return; }
        if (!res.ok) { authError.textContent = 'Ошибка регистрации'; return; }

        // Теперь login() вызовется без ошибок!
        await login();
    } catch (e) {
        console.error("Реальная ошибка:", e); // Чтобы в будущем видеть такие баги в консоли
        authError.textContent = 'Сеть недоступна';
    }
}

// ─── Вкладки ──────────────────────────────────────────────────
document.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', (e) => {
        document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));

        e.target.classList.add('active');
        document.getElementById(e.target.dataset.tab).classList.add('active');
    });
});

// ─── Игровая логика ───────────────────────────────────────────
document.getElementById('newBotGame').addEventListener('click', () => createGame('bot'));
document.getElementById('newHumanGame').addEventListener('click', () => createGame('human'));
document.getElementById('refreshAvailable').addEventListener('click', loadAvailableGames);
document.getElementById('load').addEventListener('click', loadGame);
document.getElementById('copy').addEventListener('click', () => {
    if (gameIdEl.value) navigator.clipboard.writeText(gameIdEl.value);
});

async function createGame(type) {
    stopPolling();
    const res = await fetchApi('/game', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ opponentType: type })
    });
    if (res.ok) {
        game = await res.json();
        availableGamesBlock.classList.add('hidden');
        renderGame();
        startPolling(game.id);
    }
}

async function loadAvailableGames() {
    const res = await fetchApi('/game/available');
    if (!res.ok) return;

    const games = await res.json();
    availableGamesList.innerHTML = '';

    if (games.length === 0) {
        availableGamesList.innerHTML = '<p class="text-muted">Нет доступных игр.</p>';
    } else {
        games.forEach(g => {
            const div = document.createElement('div');
            div.className = 'game-item';
            div.innerHTML = `<span>Игра: ${g.id.substring(0,8)}...</span>`;
            const btn = document.createElement('button');
            btn.className = 'btn btn-primary btn-sm';
            btn.textContent = 'Присоединиться';
            btn.onclick = () => joinGame(g.id);
            div.appendChild(btn);
            availableGamesList.appendChild(div);
        });
    }
    availableGamesBlock.classList.remove('hidden');
}

async function joinGame(id) {
    stopPolling();
    const res = await fetchApi(`/game/${id}/join`, { method: 'POST' });
    if (res.ok) {
        game = await res.json();
        availableGamesBlock.classList.add('hidden');
        renderGame();
        startPolling(game.id);
    }
}

async function loadGame() {
    const id = gameIdEl.value.trim();
    if (!id) return;
    stopPolling();

    const res = await fetchApi(`/game/${id}`);
    if (res.ok) {
        game = await res.json();
        availableGamesBlock.classList.add('hidden');
        renderGame();
        startPolling(game.id);
    } else {
        statusEl.textContent = 'Игра не найдена';
    }
}

async function makeMove(row, col) {
    if (busy || !game || game.board[row][col] !== 0 || game.currentPlayerId !== currentUserId) return;

    const mySymbol = game.player1Id === currentUserId ? game.player1Symbol : game.player2Symbol;
    const newBoard = game.board.map(r => [...r]);
    newBoard[row][col] = mySymbol;

    busy = true;
    const res = await fetchApi(`/game/${game.id}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ id: game.id, board: newBoard, status: game.status })
    });

    if (res.ok) {
        game = await res.json();
        renderGame();
    } else {
        const p = await res.json().catch(() => null);
        statusEl.textContent = p?.detail ?? 'Ошибка хода';
    }
    busy = false;
}

function startPolling(gameId) {
    stopPolling();
    pollTimer = setInterval(async () => {
        if (!game || game.id !== gameId || busy) return;
        if (game.status === 'Win' || game.status === 'Draw') { stopPolling(); return; }

        const res = await fetchApi(`/game/${gameId}`);
        if (res.ok) {
            const freshGame = await res.json();
            if (JSON.stringify(freshGame) !== JSON.stringify(game)) {
                game = freshGame;
                renderGame();
                if (game.status === 'Win' || game.status === 'Draw') stopPolling();
            }
        }
    }, 2000);
}

function stopPolling() {
    if (pollTimer) { clearInterval(pollTimer); pollTimer = null; }
}

function renderGame() {
    if (!game) return;
    gameIdEl.value = game.id;

    const isMyTurn = game.currentPlayerId === currentUserId;
    const isGameOver = game.status !== 'InProgress';

    boardEl.innerHTML = '';
    for (let i = 0; i < 3; i++) {
        for (let j = 0; j < 3; j++) {
            const btn = document.createElement('button');
            btn.className = 'cell';
            const v = game.board[i][j];
            if (v === 1) { btn.textContent = '×'; btn.classList.add('cross'); }
            else if (v === 2) { btn.textContent = '○'; btn.classList.add('zero'); }

            btn.disabled = v !== 0 || !isMyTurn || isGameOver;
            btn.addEventListener('click', () => makeMove(i, j));
            boardEl.appendChild(btn);
        }
    }

    statusEl.className = 'status-badge';
    if (game.status === 'WaitingForPlayers') {
        statusEl.textContent = 'Ожидание игрока...';
    } else if (game.status === 'Win') {
        if (game.winnerId === currentUserId) {
            statusEl.textContent = 'Вы победили!';
            statusEl.classList.add('status-win');
        } else {
            statusEl.textContent = 'Вы проиграли';
            statusEl.classList.add('status-loss');
        }
    } else if (game.status === 'Draw') {
        statusEl.textContent = 'Ничья';
    } else {
        statusEl.textContent = isMyTurn ? 'Ваш ход' : 'Ход соперника...';
    }
}

// ─── История и Лидерборд ──────────────────────────────────────
document.getElementById('loadHistoryBtn').addEventListener('click', async () => {
    const res = await fetchApi('/game/history');
    if (!res.ok) return;

    const games = await res.json();
    const list = document.getElementById('historyList');
    list.innerHTML = '';

    if (games.length === 0) {
        list.innerHTML = '<p class="text-muted">Вы еще не завершили ни одной игры.</p>';
        return;
    }

    games.forEach(g => {
        const div = document.createElement('div');
        div.className = 'history-item';
        const date = new Date(g.createdAt).toLocaleString();
        const result = g.status === 'Draw' ? 'Ничья' : (g.winnerId === currentUserId ? 'Победа' : 'Поражение');
        div.innerHTML = `<span><b>${result}</b> (${date})</span> <span class="text-muted">ID: ${g.id.substring(0,8)}...</span>`;
        list.appendChild(div);
    });
});

document.getElementById('loadLeaderboardBtn').addEventListener('click', async () => {
    const res = await fetchApi('/game/leaderboard?count=10');
    if (!res.ok) return;

    const players = await res.json();
    const tbody = document.getElementById('leaderboardBody');
    tbody.innerHTML = '';

    players.forEach((p, index) => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${index + 1}</td>
            <td>${p.login}</td>
            <td>${p.winRatio.toFixed(2)}</td>
        `;
        tbody.appendChild(tr);
    });
});