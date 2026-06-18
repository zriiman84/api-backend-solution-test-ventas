// Recaptura dirigida: dashboard (paneles corregidos) + alertas firing + alertmanager.
const { chromium } = require('playwright');
const path = require('path');
const OUT = path.join(__dirname, 'report-assets');

async function shoot(page, name, url, opts = {}) {
  try {
    await page.goto(url, { waitUntil: 'networkidle', timeout: 30000 });
    await page.waitForTimeout(opts.wait || 3000);
    await page.screenshot({ path: path.join(OUT, name), fullPage: opts.fullPage || false });
    console.log('OK   ' + name);
  } catch (e) { console.log('FAIL ' + name + ' :: ' + e.message.split('\n')[0]); }
}

(async () => {
  const browser = await chromium.launch({ args: ['--no-sandbox'] });
  const ctx = await browser.newContext({ viewport: { width: 1600, height: 900 }, ignoreHTTPSErrors: true });
  const page = await ctx.newPage();

  // Prometheus alerts (TargetDown firing) y AlertManager
  await shoot(page, 'prometheus-alerts.png', 'http://localhost:9090/alerts');
  await shoot(page, 'alertmanager.png', 'http://localhost:9093/#/alerts', { wait: 3500 });

  // Grafana login + dashboard
  try {
    await page.goto('http://localhost:3000/login', { waitUntil: 'networkidle', timeout: 30000 });
    await page.fill('input[name="user"]', 'admin');
    await page.fill('input[name="password"]', 'admin');
    await page.click('button[type="submit"]');
    await page.waitForTimeout(4000);
  } catch (e) { console.log('FAIL login :: ' + e.message.split('\n')[0]); }

  await shoot(page, 'grafana-dashboard-red.png',
    'http://localhost:3000/d/api-red?kiosk&from=now-15m&to=now&refresh=5s',
    { wait: 7000, fullPage: true });

  await browser.close();
  console.log('DONE');
})();
