const storageKey = 'energift_api_base_url';

const elements = {
  apiBaseUrl: document.getElementById('apiBaseUrl'),
  saveConfigButton: document.getElementById('saveConfigButton'),
  loadConsumptionsButton: document.getElementById('loadConsumptionsButton'),
  loadRankingButton: document.getElementById('loadRankingButton'),
  clearLogButton: document.getElementById('clearLogButton'),
  filterForm: document.getElementById('filterForm'),
  consumptionForm: document.getElementById('consumptionForm'),
  coinsForm: document.getElementById('coinsForm'),
  goalForm: document.getElementById('goalForm'),
  rankingForm: document.getElementById('rankingForm'),
  consumptionTableBody: document.getElementById('consumptionTableBody'),
  rankingList: document.getElementById('rankingList'),
  apiLog: document.getElementById('apiLog'),
  coinsResult: document.getElementById('coinsResult')
};

function getBaseUrl() {
  return (elements.apiBaseUrl.value || '').trim().replace(/\/$/, '');
}

function buildUrl(path, queryParams = {}) {
  const base = getBaseUrl();
  const url = new URL(`${base}${path}`, window.location.origin);

  Object.entries(queryParams).forEach(([key, value]) => {
    if (value !== '' && value !== null && value !== undefined) {
      url.searchParams.append(key, value);
    }
  });

  return url.toString();
}

function saveBaseUrl() {
  localStorage.setItem(storageKey, elements.apiBaseUrl.value.trim());
  writeLog('Configuração salva', { baseUrl: elements.apiBaseUrl.value.trim() || 'mesma origem' }, true);
}

function loadBaseUrl() {
  const saved = localStorage.getItem(storageKey);
  if (saved) {
    elements.apiBaseUrl.value = saved;
  }
}

function formToJson(form) {
  const data = Object.fromEntries(new FormData(form).entries());

  Object.keys(data).forEach((key) => {
    if (data[key] === '') {
      data[key] = null;
      return;
    }

    const input = form.querySelector(`[name="${key}"]`);
    if (!input) return;

    if (input.type === 'number') {
      data[key] = Number(data[key]);
    }
  });

  return data;
}

async function request(path, { method = 'GET', body = null, query = null } = {}) {
  const url = buildUrl(path, query || {});

  const config = {
    method,
    headers: {
      'Accept': 'application/json'
    }
  };

  if (body) {
    config.headers['Content-Type'] = 'application/json';
    config.body = JSON.stringify(body);
  }

  const response = await fetch(url, config);
  const rawText = await response.text();
  let payload = rawText;

  try {
    payload = rawText ? JSON.parse(rawText) : null;
  } catch {
    payload = rawText;
  }

  if (!response.ok) {
    throw new Error(typeof payload === 'string' ? payload : JSON.stringify(payload, null, 2));
  }

  writeLog(`${method} ${url}`, payload, true);
  return payload;
}

function writeLog(title, payload, success = true) {
  const stamp = new Date().toLocaleString('pt-BR');
  const serialized = typeof payload === 'string' ? payload : JSON.stringify(payload, null, 2);
  elements.apiLog.textContent = `[${stamp}] ${title}\n\n${serialized}`;
  elements.apiLog.className = success ? 'api-log status-success' : 'api-log status-error';
}

function normalizeArrayPayload(payload) {
  if (Array.isArray(payload)) return payload;
  if (payload && Array.isArray(payload.items)) return payload.items;
  if (payload && Array.isArray(payload.data)) return payload.data;
  if (payload && Array.isArray(payload.results)) return payload.results;
  return [];
}

function readAny(obj, keys, fallback = '-') {
  for (const key of keys) {
    if (obj && obj[key] !== undefined && obj[key] !== null) return obj[key];
  }
  return fallback;
}

function renderConsumptions(payload) {
  const rows = normalizeArrayPayload(payload);

  if (!rows.length) {
    elements.consumptionTableBody.innerHTML = '<tr><td colspan="6" class="empty-state">Nenhum consumo encontrado.</td></tr>';
    return;
  }

  elements.consumptionTableBody.innerHTML = rows.map((item) => {
    const referencia = readAny(item, ['referencia', 'Referencia']);
    const usuarioId = readAny(item, ['usuarioId', 'UsuarioId']);
    const imovelId = readAny(item, ['imovelId', 'ImovelId']);
    const kwh = readAny(item, ['kwh', 'Kwh']);
    const valor = readAny(item, ['valor', 'Valor']);
    const wattCoins = readAny(item, ['wattCoins', 'WattCoins', 'coins', 'Coins']);

    return `
      <tr>
        <td>${formatDate(referencia)}</td>
        <td>${usuarioId}</td>
        <td>${imovelId}</td>
        <td>${formatNumber(kwh)}</td>
        <td>${formatCurrency(valor)}</td>
        <td>${formatNumber(wattCoins)}</td>
      </tr>
    `;
  }).join('');
}

function renderRanking(payload) {
  const rows = normalizeArrayPayload(payload);

  if (!rows.length) {
    elements.rankingList.innerHTML = '<li class="empty-state">Nenhum ranking encontrado.</li>';
    return;
  }

  elements.rankingList.innerHTML = rows.map((item, index) => {
    const user = readAny(item, ['usuarioNome', 'UsuarioNome', 'nome', 'Nome', 'usuarioId', 'UsuarioId'], `Usuário ${index + 1}`);
    const score = readAny(item, ['wattCoins', 'WattCoins', 'score', 'Score', 'pontuacao', 'Pontuacao']);
    const extra = readAny(item, ['economiaPercentual', 'EconomiaPercentual', 'percentReduction', 'PercentReduction'], null);

    return `
      <li class="ranking-item">
        <strong>${index + 1}º lugar:</strong> ${user}
        <div>Pontuação: ${formatNumber(score)}</div>
        ${extra !== null && extra !== '-' ? `<div>Economia: ${formatNumber(extra)}%</div>` : ''}
      </li>
    `;
  }).join('');
}

function formatDate(value) {
  if (!value || value === '-') return '-';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleDateString('pt-BR');
}

function formatCurrency(value) {
  if (value === '-' || value === null || value === undefined || Number.isNaN(Number(value))) return '-';
  return Number(value).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

function formatNumber(value) {
  if (value === '-' || value === null || value === undefined || Number.isNaN(Number(value))) return '-';
  return Number(value).toLocaleString('pt-BR', { maximumFractionDigits: 2 });
}

async function loadConsumptions() {
  try {
    const filters = formToJson(elements.filterForm);
    const payload = await request('/api/Consumo', { query: filters });
    renderConsumptions(payload);
  } catch (error) {
    writeLog('Erro ao buscar histórico de consumo', error.message, false);
    elements.consumptionTableBody.innerHTML = `<tr><td colspan="6" class="empty-state">${error.message}</td></tr>`;
  }
}

async function createConsumption(event) {
  event.preventDefault();

  try {
    const body = formToJson(elements.consumptionForm);
    const payload = await request('/api/Consumo', { method: 'POST', body });
    writeLog('Consumo cadastrado com sucesso', payload, true);
    elements.consumptionForm.reset();
    await loadConsumptions();
  } catch (error) {
    writeLog('Erro ao cadastrar consumo', error.message, false);
  }
}

async function calculateCoins(event) {
  event.preventDefault();

  try {
    const body = formToJson(elements.coinsForm);
    const payload = await request('/api/Consumo/calculate-coins', { method: 'POST', body });
    const awarded = readAny(payload, ['awarded', 'Awarded'], 0);
    elements.coinsResult.textContent = `WattCoins geradas: ${formatNumber(awarded)}`;
  } catch (error) {
    elements.coinsResult.textContent = `Falha ao calcular: ${error.message}`;
    elements.coinsResult.className = 'highlight-box status-error';
    writeLog('Erro ao calcular WattCoins', error.message, false);
    return;
  }

  elements.coinsResult.className = 'highlight-box';
}

async function createGoal(event) {
  event.preventDefault();

  try {
    const body = formToJson(elements.goalForm);
    const payload = await request('/api/Goal', { method: 'POST', body });
    writeLog('Meta cadastrada com sucesso', payload, true);
    elements.goalForm.reset();
  } catch (error) {
    writeLog('Erro ao cadastrar meta', error.message, false);
  }
}

async function loadRanking() {
  try {
    const { period } = formToJson(elements.rankingForm);
    const payload = await request('/api/Ranking', { query: { period } });
    renderRanking(payload);
  } catch (error) {
    writeLog('Erro ao buscar ranking', error.message, false);
    elements.rankingList.innerHTML = `<li class="empty-state">${error.message}</li>`;
  }
}

function clearLog() {
  elements.apiLog.textContent = 'Aguardando interações...';
  elements.apiLog.className = 'api-log';
}

function wireEvents() {
  elements.saveConfigButton.addEventListener('click', saveBaseUrl);
  elements.loadConsumptionsButton.addEventListener('click', loadConsumptions);
  elements.loadRankingButton.addEventListener('click', loadRanking);
  elements.clearLogButton.addEventListener('click', clearLog);
  elements.consumptionForm.addEventListener('submit', createConsumption);
  elements.coinsForm.addEventListener('submit', calculateCoins);
  elements.goalForm.addEventListener('submit', createGoal);
  elements.rankingForm.addEventListener('change', loadRanking);
}

function bootstrap() {
  loadBaseUrl();
  wireEvents();
  loadRanking();
}

bootstrap();
