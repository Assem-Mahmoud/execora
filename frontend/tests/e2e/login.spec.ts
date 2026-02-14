import { test, expect } from '@playwright/test';

test.describe('Login Flow', () => {
  test('should display login page', async ({ page }) => {
    await page.goto('/login');

    // Check if login form is visible
    await expect(page.locator('form')).toBeVisible();
    await expect(page.locator('input#email')).toBeVisible();
    await expect(page.locator('input#password')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('should show validation errors for empty form', async ({ page }) => {
    await page.goto('/login');

    // Try to submit empty form
    await page.click('button[type="submit"]');

    // Check error messages
    await expect(page.locator('nz-form-control')).toContainText('Please enter a valid email address');
    await expect(page.locator('nz-form-control')).toContainText('Password must be at least 12 characters long');
  });

  test('should show validation error for invalid email', async ({ { page } }) => {
    await page.goto('/login');

    // Fill with invalid email
    await page.fill('input#email', 'invalid-email');
    await page.fill('input#password', 'Password123!');

    // Try to submit
    await page.click('button[type="submit"]');

    // Check error message
    await expect(page.locator('nz-form-control')).toContainText('Please enter a valid email address');
  });

  test('should show validation error for weak password', async ({ page }) => {
    await page.goto('/login');

    // Fill with weak password
    await page.fill('input#email', 'test@example.com');
    await page.fill('input#password', 'short');

    // Try to submit
    await page.click('button[type="submit"]');

    // Check error message
    await expect(page.locator('nz-form-control')).toContainText('Password must be at least 12 characters long');
  });

  test('should navigate to register page', async ({ page }) => {
    await page.goto('/login');

    // Click create account button
    await page.click('button:has-text("Create Account")');

    // Should navigate to register page
    await expect(page).toHaveURL('/register');
  });

  test('should navigate to forgot password page', async ({ page }) => {
    await page.goto('/login');

    // Click forgot password link
    await page.click('a:has-text("Forgot Password?")');

    // Should navigate to forgot password page
    await expect(page).toHaveURL('/auth/forgot-password');
  });

  test('should login with valid credentials', async ({ page }) => {
    // Note: This test requires a backend to be running
    // with a test user account

    await page.goto('/login');

    // Fill with valid credentials
    await page.fill('input#email', 'test@example.com');
    await page.fill('input#password', 'Password123!');

    // Click remember me checkbox
    await page.check('input[type="checkbox"]');

    // Submit form
    await page.click('button[type="submit"]');

    // Wait for navigation (success)
    // This will fail without a backend
    await expect(page).toHaveURL('/dashboard');
  });

  test('should show error for invalid credentials', async ({ page }) => {
    // Note: This test requires a backend to be running

    await page.goto('/login');

    // Fill with invalid credentials
    await page.fill('input#email', 'wrong@example.com');
    await page.fill('input#password', 'WrongPassword123!');

    // Submit form
    await page.click('button[type="submit"]');

    // Should show error message
    await expect(page.locator('.ant-message')).toContainText('Login failed');
  });

  test('should handle account lockout after multiple failed attempts', async ({ page }) => {
    // Note: This test requires a backend to be running

    await page.goto('/login');

    // Try multiple times with wrong password
    for (let i = 0; i < 5; i++) {
      await page.fill('input#email', 'test@example.com');
      await page.fill('input#password', 'WrongPassword123!');
      await page.click('button[type="submit"]');

      // Wait a bit between attempts
      await page.waitForTimeout(1000);
    }

    // Should show account locked message
    await expect(page.locator('.ant-message')).toContainText('account is temporarily locked');
  });

  test('should redirect to dashboard if already logged in', async ({ page }) => {
    // Note: This test requires a backend to be running
    // and needs to simulate being logged in

    // Set the access token in localStorage
    await page.evaluate(() => {
      localStorage.setItem('execora_access_token', 'fake-token');
    });

    // Navigate to login page
    await page.goto('/login');

    // Should redirect to dashboard
    await expect(page).toHaveURL('/dashboard');
  });
});