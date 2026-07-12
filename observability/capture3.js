// Recaptura de Explore (Grafana 11 usa el parámetro 'panes').
const { chromium } = require('playwright');
const path = require('path');
const OUT = path.join(__dirname, 'report-assets');

(async () => {
  const browser = await chromium.launch({ args: ['--no-sandbox'] });
  const ctx = await browser.newContext({ viewport: { width: 1600, height: 900 }, ignoreHTTPSErrors: true });
  const page = await ctx.newPage();

  // login
  await page.goto('http://localhost:3000/login', { waitUntil: 'networkidle', timeout: 30000 });
  await page.fill('input[name="user"]', 'admin');
  await page.fill('input[name="password"]', 'admin');
  await page.click('button[type="submit"]');
  await page.waitForTimeout(4000);

  async function explore(name, panesObj) {
    try {
      const panes = encodeURIComponent(JSON.stringify(panesObj));
      await page.goto(`http://localhost:3000/explore?schemaVersion=1&panes=${panes}&orgId=1`,
        { waitUntil: 'networkidle', timeout: 30000 });
      await page.waitForTimeout(7000);
      await page.screenshot({ path: path.join(OUT, name) });
      console.log('OK   ' + name);
    } catch (e) { console.log('FAIL ' + name + ' :: ' + e.message.split('\n')[0]); }
  }

  // Loki (logs)
  await explore('grafana-loki-logs.png', {
    lk: {
      datasource: 'loki',
      queries: [{ refId: 'A', datasource: { type: 'loki', uid: 'loki' },
                  expr: '{service_name="api-solution-test-ventas"}', queryType: 'range' }],
      range: { from: 'now-30m', to: 'now' },
    },
  });

  // Tempo (trazas, TraceQL)
  await explore('grafana-tempo-search.png', {
    tp: {
      datasource: 'tempo',
      queries: [{ refId: 'A', datasource: { type: 'tempo', uid: 'tempo' },
                  queryType: 'traceql', query: '{resource.service.name="api-solution-test-ventas"}', limit: 20 }],
      range: { from: 'now-1h', to: 'now' },
    },
  });

  await browser.close();
  console.log('DONE');
})();
