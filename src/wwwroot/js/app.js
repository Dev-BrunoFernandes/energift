// Gerenciamento de Configuração e Estado
const state = {
    apiBaseUrl: localStorage.getItem('apiBaseUrl') || '',
    userTotalCoins: 0
};

// Elementos do DOM
const elements = {
    apiBaseUrlInput: document.getElementById('apiBaseUrl'),
    saveConfigButton: document.getElementById('saveConfigButton'),
    configToggle: document.getElementById('configToggle'),
    configPanel: document.getElementById('configPanel'),
    consumptionForm: document.getElementById('consumptionForm'),
    coinsForm: document.getElementById('coinsForm'),
    loadConsumptionsButton: document.getElementById('loadConsumptionsButton'),
    loadRankingButton: document.getElementById('loadRankingButton'),
    consumptionTableBody: document.getElementById('consumptionTableBody'),
    rankingList: document.getElementById('rankingList'),
    coinsResult: document.getElementById('coinsResult'),
    apiLog: document.getElementById('apiLog'),
    clearLogButton: document.getElementById('clearLogButton'),
    userTotalCoins: document.getElementById('userTotalCoins'),
    toast: document.getElementById('toast'),
    toastMessage: document.getElementById('toastMessage'),
    toastIcon: document.getElementById('toastIcon')
};

// Inicialização
document.addEventListener('DOMContentLoaded', () => {
    elements.apiBaseUrlInput.value = state.apiBaseUrl;
    setupEventListeners();
    log('Sistema inicializado com sucesso.');
});

function setupEventListeners() {
    // Configurações
    elements.configToggle.addEventListener('click', () => elements.configPanel.classList.toggle('hidden'));
    elements.saveConfigButton.addEventListener('click', saveConfig);
    elements.clearLogButton.addEventListener('click', () => elements.apiLog.innerHTML = '> Logs limpos.');

    // Formulários
    elements.consumptionForm.addEventListener('submit', handleConsumptionSubmit);
    elements.coinsForm.addEventListener('submit', handleCoinsCalculate);

    // Listagens
    elements.loadConsumptionsButton.addEventListener('click', loadConsumptions);
    elements.loadRankingButton.addEventListener('click', loadRanking);
}

// Funções de API
async function apiFetch(endpoint, options = {}) {
    const url = `${state.apiBaseUrl}${endpoint}`;
    const defaultOptions = {
        headers: { 'Content-Type': 'application/json' },
        ...options
    };

    try {
        const response = await fetch(url, defaultOptions);
        const data = await response.json();
        
        if (!response.ok) {
            throw new Error(data.error || data.message || `Erro HTTP: ${response.status}`);
        }
        
        log(`Sucesso: ${options.method || 'GET'} ${endpoint}`);
        return data;
    } catch (error) {
        log(`Erro: ${error.message}`, 'error');
        showToast(error.message, 'error');
        throw error;
    }
}

// Handlers
async function handleConsumptionSubmit(e) {
    e.preventDefault();
    const formData = new FormData(e.target);
    const payload = Object.fromEntries(formData.entries());
    
    // Converter tipos
    payload.usuarioId = parseInt(payload.usuarioId);
    payload.imovelId = parseInt(payload.imovelId);
    payload.kwh = parseFloat(payload.kwh);
    payload.valor = parseFloat(payload.valor);

    try {
        await apiFetch('/api/consumo', {
            method: 'POST',
            body: JSON.stringify(payload)
        });
        showToast('Consumo registrado com sucesso!', 'success');
        e.target.reset();
        loadConsumptions();
        loadRanking();
    } catch (err) {}
}

async function handleCoinsCalculate(e) {
    e.preventDefault();
    const formData = new FormData(e.target);
    const payload = Object.fromEntries(formData.entries());
    
    const params = new URLSearchParams({
        kwh: payload.kwh,
        valor: 0, // Valor não afeta cálculo de moedas nesta versão
        previousKwh: payload.previousKwh || 0
    });

    try {
        const result = await apiFetch(`/api/watcoin/calculate?${params}`);
        elements.coinsResult.innerText = `${result.wattCoins.toFixed(2)} WC`;
        showToast('Cálculo realizado!', 'info');
    } catch (err) {}
}

async function loadConsumptions() {
    try {
        const data = await apiFetch('/api/consumo?usuarioId=1&pageSize=10');
        renderConsumptions(data.items || []);
    } catch (err) {}
}

async function loadRanking() {
    try {
        const data = await apiFetch('/api/ranking?period=monthly');
        renderRanking(data || []);
    } catch (err) {}
}

// Renderização
function renderConsumptions(items) {
    if (items.length === 0) {
        elements.consumptionTableBody.innerHTML = '<tr><td colspan="5" class="px-6 py-10 text-center text-gray-500 italic">Nenhum dado encontrado.</td></tr>';
        return;
    }

    elements.consumptionTableBody.innerHTML = items.map(item => `
        <tr class="hover:bg-gray-50 transition-colors">
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">${new Date(item.referencia).toLocaleDateString()}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">Imóvel #${item.imovelId}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">${item.kwh.toFixed(2)} kWh</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">R$ ${item.valor.toFixed(2)}</td>
            <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-emerald-100 text-emerald-800">
                    +${item.wattCoins.toFixed(2)} WC
                </span>
            </td>
        </tr>
    `).join('');
}

function renderRanking(items) {
    if (items.length === 0) {
        elements.rankingList.innerHTML = '<li class="py-3 text-center text-gray-500 text-sm italic">Nenhum dado no ranking.</li>';
        return;
    }

    elements.rankingList.innerHTML = items.map((item, index) => `
        <li class="py-3 flex items-center justify-between">
            <div class="flex items-center">
                <span class="flex-shrink-0 w-8 h-8 flex items-center justify-center rounded-full ${index < 3 ? 'bg-amber-100 text-amber-700 font-bold' : 'bg-gray-100 text-gray-500'} text-sm mr-3">
                    ${index + 1}
                </span>
                <span class="text-sm font-medium text-gray-900">Usuário #${item.usuarioId}</span>
            </div>
            <span class="text-sm font-bold text-emerald-600">${item.totalWattCoins.toFixed(2)} WC</span>
        </li>
    `).join('');
}

// Helpers
function saveConfig() {
    state.apiBaseUrl = elements.apiBaseUrlInput.value.trim();
    localStorage.setItem('apiBaseUrl', state.apiBaseUrl);
    showToast('Configuração salva!', 'success');
    elements.configPanel.classList.add('hidden');
    log(`URL da API alterada para: ${state.apiBaseUrl || '(origem local)'}`);
}

function log(msg, type = 'info') {
    const timestamp = new Date().toLocaleTimeString();
    const color = type === 'error' ? 'text-red-400' : 'text-emerald-400';
    elements.apiLog.innerHTML += `<div class="${color} mb-1">
        <span class="opacity-50">[${timestamp}]</span> ${msg}
    </div>`;
    elements.apiLog.scrollTop = elements.apiLog.scrollHeight;
}

function showToast(message, type = 'info') {
    const colors = {
        success: 'bg-emerald-600',
        error: 'bg-red-600',
        info: 'bg-blue-600'
    };
    
    elements.toast.className = `fixed bottom-5 right-5 flex items-center space-x-3 px-6 py-3 rounded-lg shadow-2xl z-50 transition-all duration-300 ${colors[type]}`;
    elements.toastMessage.innerText = message;
    
    // Icon
    const icons = {
        success: '<i class="fas fa-check-circle"></i>',
        error: '<i class="fas fa-exclamation-triangle"></i>',
        info: '<i class="fas fa-info-circle"></i>'
    };
    elements.toastIcon.innerHTML = icons[type];

    // Show
    elements.toast.classList.remove('translate-y-20', 'opacity-0');
    setTimeout(() => {
        elements.toast.classList.add('translate-y-20', 'opacity-0');
    }, 3000);
}
