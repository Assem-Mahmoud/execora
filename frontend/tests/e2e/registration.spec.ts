import { test, expect } from '@playwright/test';

test.describe('User Registration Flow', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to registration page
    await page.goto('/register');
  });

  test('should show registration form with all required fields', async ({ page }) => {
    // Check that all form fields are present
    await expect(page.getByLabel('Email')).toBeVisible();
    await expect(page.getByLabel('Password')).toBeVisible();
    await expect(page.getByLabel('First Name')).toBeVisible();
    await expect(page.getByLabel('Last Name')).toBeVisible();
    await expect(page.getByLabel('Organization Name')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Register' })).toBeVisible();
  });

  test('should validate required fields on submit', async ({ page }) => {
    // Click register button without filling fields
    await page.click('button:has-text("Register")');

    // Check validation errors
    await expect(page.getByText('Email is required')).toBeVisible();
    await expect(page.getByText('Password is required')).toBeVisible();
    await expect(page.getByText('First name is required')).toBeVisible();
    await expect(page.getByText('Last name is required')).toBeVisible();
    await expect(page.getByText('Organization name is required')).toBeVisible();
  });

  test('should validate email format', async ({ page }) => {
    // Fill form with invalid email
    await page.fill('input[name="email"]', 'invalid-email');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName"]', 'Test Org');

    await page.click('button:has-text("Register")');

    // Check email validation error
    await expect(page.getByText('Email format is invalid')).toBeVisible();
  });

  test('should validate password requirements', async ({ page }) => {
    // Test short password
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'short');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName"]', 'Test Org');

    await page.click('button:has-text("Register")');

    await expect(page.getByText('Password must be at least 12 characters long')).toBeVisible();
  });

  test('should validate password complexity requirements', async ({ page }) => {
    // Test missing uppercase
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName"]', 'Test Org');

    await page.click('button:has-text("Register")');

    await expect(page.getByText('Password must contain at least one uppercase letter')).toBeVisible();
  });

  test('should validate first name length', async ({ page }) => {
    // Test too long first name
    const longName = new Array(102).fill('a').join('');

    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', longName);
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName"]', 'Test Org');

    await page.click('button:has-text("Register")');

    await expect(page.getByText('First name must be less than 100 characters')).toBeVisible();
  });

  test('should validate last name length', async ({ page }) => {
    // Test too long last name
    const longName = new Array(102).fill('a').join('');

    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', longName);
    await page.fill('input[name="organizationName"]', 'Test Org');

    await page.click('button:has-text("Register")');

    await expect(page.getByText('Last name must be less than 100 characters')).toBeVisible();
  });

  test('should validate organization name requirements', async ({ page }) => {
    // Test empty organization name
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName']', '');

    await page.click('button:has-text("Register")');

    await expect(page.getByText('Organization name is required')).toBeVisible();
  });

  test('should validate organization name length', async ({ page }) => {
    // Test too short organization name
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName']', 'A');

    await page.click('button:has-text("Register")');

    await expect(page.getByText('Organization name must be at least 3 characters long')).toBeVisible();
  });

  test('should validate organization name characters', async ({ page }) => {
    // Test invalid characters in organization name
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName']', 'Test@Org');

    await page.click('button:has-text("Register")');

    await expect(page.getByText('Organization name can only contain letters, numbers, spaces, and hyphens')).toBeVisible();
  });

  test('should show loading state during registration', async ({ page }) => {
    // Fill form with valid data
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName"]', 'Test Organization');

    // Mock a slow API response
    await page.route('**/api/auth/register', route => {
      route.continue();
    });

    // Click register button
    const registerButton = page.getByRole('button', { name: 'Register' });
    await registerButton.click();

    // Check that loading state is shown
    await expect(registerButton).toBeDisabled();
    await expect(page.getByText('Registering...')).toBeVisible();
  });

  test('should redirect to login after successful registration', async ({ page }) => {
    // This test will fail until registration is implemented
    // Fill form with valid data
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName"]', 'Test Organization');

    // Mock successful API response
    await page.route('**/api/auth/register', route => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          userId: 'user-id-123',
          tenantId: 'tenant-id-123',
          role: 'TenantAdmin'
        })
      });
    });

    // Click register button
    await page.click('button:has-text("Register")');

    // Check that page redirects to login
    await expect(page).toHaveURL('/login');
    await expect(page.getByText('Registration successful!')).toBeVisible();
  });

  test('should show error message for duplicate email', async ({ page }) => {
    // This test will fail until registration is implemented
    // Fill form with valid data
    await page.fill('input[name="email"]', 'duplicate@example.com');
    await page.fill('input[name="password"]', 'Password123!');
    await page.fill('input[name="firstName"]', 'John');
    await page.fill('input[name="lastName"]', 'Doe');
    await page.fill('input[name="organizationName"]', 'Test Organization');

    // Mock conflict response
    await page.route('**/api/auth/register', route => {
      route.fulfill({
        status: 409,
        contentType: 'application/problem+json',
        body: JSON.stringify({
          type: 'https://example.com/probs/email-already-registered',
          title: 'Email already registered',
          detail: 'An account with this email already exists'
        })
      });
    });

    // Click register button
    await page.click('button:has-text("Register")');

    // Check error message
    await expect(page.getByText('Email already registered')).toBeVisible();
  });

  test('should have link to login page', async ({ page }) => {
    // Check if there's a link to login page
    const loginLink = page.getByRole('link', { name: 'Already have an account? Login' });
    await expect(loginLink).toBeVisible();
    await expect(loginLink).toHaveAttribute('href', '/login');
  });

  test('should show password strength indicator', async ({ page }) => {
    // Fill password field
    await page.fill('input[name="password"]', 'password');

    // Check password strength indicator
    await expect(page.getByText('Weak')).toBeVisible();

    // Update with stronger password
    await page.fill('input[name="password"]', 'Password123!');

    // Check password strength indicator
    await expect(page.getByText('Medium')).toBeVisible();
  });

  test('should show tooltips for password requirements', async ({ page }) => {
    // Hover over password field to show tooltip
    const passwordField = page.locator('input[name="password"]');
    await passwordField.hover();

    // Check password requirements tooltip
    await expect(page.getByText('Must be at least 12 characters')).toBeVisible();
    await expect(page.getByText('Include uppercase, lowercase, number, and special character')).toBeVisible();
  });

  test('should navigate to register page from login page', async ({ page }) => {
    // Start on login page
    await page.goto('/login');

    // Click register link
    await page.click('text="Register"');

    // Should be on registration page
    await expect(page).toHaveURL('/register');
  });
});