//const API_BASE = "https://localhost:7266";
const API_BASE = window.location.origin;
function esc(str) {
    return String(str ?? '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

async function apiFetch(url, options = {}) {
    const res = await fetch(url, {
        headers: { 'accept': '*/*' },
        ...options
    });
    if (!res.ok) {
        let msg = `Error ${res.status}`;
        try { const b = await res.json(); msg = b.message || msg; } catch { }
        throw new Error(msg);
    }
    const ct = res.headers.get('content-type') || '';
    if (ct.includes('application/json')) {
        return res.json();
    }
    if (res.status === 204) return null;
    return res.text();
}

async function apiGet(url) {
    return apiFetch(url);
}

async function apiPost(url, data) {
    return apiFetch(url, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
}

async function apiPut(url, data) {
    return apiFetch(url, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
}

async function apiDelete(url) {
    return apiFetch(url, { method: 'DELETE' });
}

function extractData(response) {
    return response?.data ?? response;
}
