import { test, expect } from '@playwright/test';

test.describe('StarkMine Application Error Check', () => {

    test.beforeEach(async ({ page }) => {
        // Log console errors during test
        page.on('console', (msg) => {
            if (msg.type() === 'error') {
                console.log(`❌ Console Error: ${msg.text()}`);
            }
        });

        // Log unhandled exceptions
        page.on('pageerror', (exception) => {
            console.log(`❌ Page Exception: ${exception}`);
        });
    });

    test('Homepage loads without errors', async ({ page }) => {
        await page.goto('/');

        // Check that the page title is correct
        await expect(page).toHaveTitle(/StarkMine/);

        // Wait for the page to be fully loaded
        await page.waitForLoadState('networkidle');

        // Check for any error elements
        const errorElements = page.locator('[data-testid="error"], .error, [class*="error"]');
        await expect(errorElements).toHaveCount(0);

        // Check that main navigation is present
        await expect(page.locator('nav, [role="navigation"]')).toBeVisible();
    });

    test('StarkMine Dashboard loads without errors', async ({ page }) => {
        await page.goto('/starkmine');

        // Wait for the dashboard to load
        await page.waitForLoadState('networkidle');

        // Check for wallet status component
        await expect(page.locator('text=MINE Balance')).toBeVisible({ timeout: 10000 });

        // Check for mining overview
        await expect(page.locator('text=Mining Overview')).toBeVisible();

        // Check for halving countdown
        await expect(page.locator('text=Next Halving')).toBeVisible();

        // Verify no error messages are displayed
        const errorElements = page.locator('[data-testid="error"], .error, [class*="error"]');
        await expect(errorElements).toHaveCount(0);
    });

    test('Miners page loads without errors', async ({ page }) => {
        await page.goto('/starkmine/miners');

        await page.waitForLoadState('networkidle');

        // Check for miners page title
        await expect(page.locator('h1, h2').filter({ hasText: /Miners?/ })).toBeVisible();

        // Check for tier buttons or sections
        await expect(page.locator('text=Basic')).toBeVisible();
        await expect(page.locator('text=Elite')).toBeVisible();
        await expect(page.locator('text=Pro')).toBeVisible();
        await expect(page.locator('text=GIGA')).toBeVisible();

        // Verify no error states
        const errorElements = page.locator('[data-testid="error"], .error, [class*="error"]');
        await expect(errorElements).toHaveCount(0);
    });

    test('Engines page loads without errors', async ({ page }) => {
        await page.goto('/starkmine/engines');

        await page.waitForLoadState('networkidle');

        // Check for engines page content
        await expect(page.locator('h1, h2').filter({ hasText: /Engine/i })).toBeVisible();

        // Check for engine types
        await expect(page.locator('text=Standard')).toBeVisible();

        // Verify no error states
        const errorElements = page.locator('[data-testid="error"], .error, [class*="error"]');
        await expect(errorElements).toHaveCount(0);
    });

    test('Station page loads without errors', async ({ page }) => {
        await page.goto('/starkmine/station');

        await page.waitForLoadState('networkidle');

        // Check for station page content
        await expect(page.locator('h1, h2').filter({ hasText: /Station/i })).toBeVisible();

        // Check for station level information
        await expect(page.locator('text=Level')).toBeVisible();

        // Verify no error states
        const errorElements = page.locator('[data-testid="error"], .error, [class*="error"]');
        await expect(errorElements).toHaveCount(0);
    });

    test('Merge page loads without errors', async ({ page }) => {
        await page.goto('/starkmine/merge');

        await page.waitForLoadState('networkidle');

        // Check for merge page content
        await expect(page.locator('h1, h2').filter({ hasText: /Merge/i })).toBeVisible();

        // Check for merge type sections
        await expect(page.locator('text=Elite').or(page.locator('text=Pro'))).toBeVisible();

        // Verify no error states
        const errorElements = page.locator('[data-testid="error"], .error, [class*="error"]');
        await expect(errorElements).toHaveCount(0);
    });

    test('Navigation between pages works', async ({ page }) => {
        await page.goto('/starkmine');

        // Test navigation to different sections
        const navLinks = [
            { text: 'Miners', url: '/starkmine/miners' },
            { text: 'Engines', url: '/starkmine/engines' },
            { text: 'Station', url: '/starkmine/station' },
            { text: 'Merge', url: '/starkmine/merge' },
        ];

        for (const link of navLinks) {
            // Click navigation link
            await page.click(`text=${link.text}`);

            // Wait for navigation
            await page.waitForLoadState('networkidle');

            // Verify URL
            expect(page.url()).toContain(link.url);

            // Check no errors occurred during navigation
            const errorElements = page.locator('[data-testid="error"], .error, [class*="error"]');
            await expect(errorElements).toHaveCount(0);
        }
    });

    test('Responsive design works', async ({ page }) => {
        await page.goto('/starkmine');

        // Test mobile viewport
        await page.setViewportSize({ width: 375, height: 667 });
        await page.waitForLoadState('networkidle');

        // Check that main content is still visible
        await expect(page.locator('text=Mining Overview')).toBeVisible();

        // Test tablet viewport
        await page.setViewportSize({ width: 768, height: 1024 });
        await page.waitForLoadState('networkidle');

        // Check content is still accessible
        await expect(page.locator('text=MINE Balance')).toBeVisible();

        // Test desktop viewport
        await page.setViewportSize({ width: 1920, height: 1080 });
        await page.waitForLoadState('networkidle');

        // Verify no errors at any viewport size
        const errorElements = page.locator('[data-testid="error"], .error, [class*="error"]');
        await expect(errorElements).toHaveCount(0);
    });

    test('Check for JavaScript errors in console', async ({ page }) => {
        const consoleErrors: string[] = [];
        const pageErrors: string[] = [];

        // Collect console errors
        page.on('console', (msg) => {
            if (msg.type() === 'error') {
                consoleErrors.push(msg.text());
            }
        });

        // Collect page errors
        page.on('pageerror', (exception) => {
            pageErrors.push(exception.toString());
        });

        // Visit all main pages
        const pages = [
            '/starkmine',
            '/starkmine/miners',
            '/starkmine/engines',
            '/starkmine/station',
            '/starkmine/merge'
        ];

        for (const pagePath of pages) {
            await page.goto(pagePath);
            await page.waitForLoadState('networkidle');

            // Wait a bit for any async operations
            await page.waitForTimeout(2000);
        }

        // Report any errors found
        if (consoleErrors.length > 0) {
            console.log('❌ Console Errors Found:');
            consoleErrors.forEach(error => console.log(`  - ${error}`));
        }

        if (pageErrors.length > 0) {
            console.log('❌ Page Errors Found:');
            pageErrors.forEach(error => console.log(`  - ${error}`));
        }

        // Test passes if no critical errors (we might want to allow some warnings)
        const criticalErrors = [...consoleErrors, ...pageErrors].filter(error =>
            !error.includes('Warning') &&
            !error.includes('Hydration') &&
            !error.includes('useAccount')
        );

        expect(criticalErrors.length).toBe(0);
    });

    test('Check accessibility basics', async ({ page }) => {
        await page.goto('/starkmine');
        await page.waitForLoadState('networkidle');

        // Check for proper heading structure
        const h1 = page.locator('h1');
        await expect(h1).toHaveCount(1); // Should have exactly one h1

        // Check for alt text on images
        const images = page.locator('img');
        const imageCount = await images.count();

        for (let i = 0; i < imageCount; i++) {
            const img = images.nth(i);
            const alt = await img.getAttribute('alt');
            expect(alt).not.toBeNull(); // All images should have alt text
        }

        // Check for aria-labels on interactive elements without text
        const buttons = page.locator('button');
        const buttonCount = await buttons.count();

        for (let i = 0; i < buttonCount; i++) {
            const button = buttons.nth(i);
            const text = await button.textContent();
            const ariaLabel = await button.getAttribute('aria-label');

            // Button should have either text content or aria-label
            expect(text?.trim() || ariaLabel).toBeTruthy();
        }
    });

}); 