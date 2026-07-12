// Captura de pantallas de las UIs de observabilidad con Playwright (headless).
// Ejecutar: cd ~/sttv/observability && NODE_PATH=$HOME/shotter/node_modules node capture.js
const { chromium } = require('playwright');
const fs = require('fs');
const path = require('path');

const OUT = path.join(__dirname, 'report-assets');
fs.mkdirSync(OUT, { recursive: true });

async function shoot(page, name, url, opts = {}) {
  try {
    await page.goto(url, { waitUntil: 'networkidle', timeout: 30000 });
    await page.waitForTimeout(opts.wait || 2500);
    await page.screenshot({ path: path.join(OUT, name), fullPage: opts.fullPage || false });
    console.log('OK   ' + name);
  } catch (e) {
    console.log('FAIL ' + name + ' :: ' + e.message.split('\n')[0]);
  }
}

(async () => {
  const browser = await chromium.launch({ args: ['--no-sandbox'] });
  const ctx = await browser.newContext({ viewport: { width: 1600, height: 900 }, ignoreHTTPSErrors: true });
  const page = await ctx.newPage();

  // ---- App + API ----
  await shoot(page, 'app-frontend.png', 'http://localhost/');
  await shoot(page, 'swagger.png', 'http://localhost:8080/swagger/index.html', { wait: 3500 });

  // ---- Prometheus ----
  await shoot(page, 'prometheus-targets.png', 'http://localhost:9090/targets');
  await shoot(page, 'prometheus-alerts.png', 'http://localhost:9090/alerts');

  // ---- AlertManager ----
  await shoot(page, 'alertmanager.png', 'http://localhost:9093/#/alerts', { wait: 3000 });

  // ---- Grafana: login ----
  try {
    await page.goto('http://localhost:3000/login', { waitUntil: 'networkidle', timeout: 30000 });
    await page.fill('input[name="user"]', 'admin');
    await page.fill('input[name="password"]', 'admin');
    await page.click('button[type="submit"]');
    await page.waitForTimeout(4000);
    console.log('OK   grafana-login');
  } catch (e) {
    console.log('FAIL grafana-login :: ' + e.message.split('\n')[0]);
  }

  // ---- Grafana: dashboard RED (kiosk) ----
  await shoot(page, 'grafana-dashboard-red.png',
    'http://localhost:3000/d/api-red?kiosk&from=now-15m&to=now&refresh=5s',
    { wait: 6000, fullPage: true });

  // ---- Grafana Explore: Loki (logs) ----
  const lokiLeft = encodeURIComponent(JSON.stringify({
    datasource: 'loki',
    queries: [{ refId: 'A', expr: '{service_name="api-solution-test-ventas"}' }],
    range: { from: 'now-15m', to: 'now' },
  }));
  await shoot(page, 'grafana-loki-logs.png',
    `http://localhost:3000/explore?schemaVersion=1&panes=&left=${lokiLeft}&orgId=1`,
    { wait: 6000 });

  // ---- Grafana Explore: Tempo (búsqueda de trazas) ----
  const tempoLeft = encodeURIComponent(JSON.stringify({
    datasource: 'tempo',
    queries: [{ refId: 'A', queryType: 'traceqlSearch', filters: [
      { id: 'service', tag: 'service.name', operator: '=', scope: 'resource', value: ['api-solution-test-ventas'], valueType: 'string' }
    ] }],
    range: { from: 'now-30m', to: 'now' },
  }));
  await shoot(page, 'grafana-tempo-search.png',
    `http://localhost:3000/explore?schemaVersion=1&panes=&left=${tempoLeft}&orgId=1`,
    { wait: 6000 });

  await browser.close();
  console.log('DONE capturas en ' + OUT);
})();
