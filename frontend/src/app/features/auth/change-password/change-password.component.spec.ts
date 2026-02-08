import { ComponentFixture, TestBed, fakeAsync } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ChangePasswordComponent } from './change-password.component';
import { of, throwError } from 'rxjs';
import { tick } from '@angular/core/testing';

describe('ChangePasswordComponent', () => {
  let component: ChangePasswordComponent;
  let fixture: ComponentFixture<ChangePasswordComponent>;
  let authService: any;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    // Create mock service
    authService = {
      isAuthenticated: false,
      changePassword: jasmine.createSpy('changePassword').and.returnValue(of({ message: 'Success' })),
      logout: jasmine.createSpy('logout').and.returnValue(of(undefined))
    };

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        ReactiveFormsModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatIconModule,
        MatSnackBarModule,
        ChangePasswordComponent
      ],
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ChangePasswordComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should redirect to login if not authenticated on init', () => {
    // Set mock to not authenticated
    authService.isAuthenticated = false;
    fixture.detectChanges();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth/login']);
  });

  it('should not redirect if authenticated on init', () => {
    // Mock the authService.isAuthenticated property
    authService.isAuthenticated = true;
    fixture.detectChanges();
    expect(routerSpy.navigate).not.toHaveBeenCalled();
  });

  it('should create form with required fields', () => {
    authService.isAuthenticated = true;
    fixture.detectChanges();

    const form = component.changePasswordForm;
    expect(form).toBeTruthy();
    expect(form.controls['currentPassword']).toBeTruthy();
    expect(form.controls['newPassword']).toBeTruthy();
    expect(form.controls['confirmPassword']).toBeTruthy();
  });

  it('should invalidate form when empty', () => {
    authService.isAuthenticated = true;
    fixture.detectChanges();

    const form = component.changePasswordForm;
    expect(form.valid).toBeFalsy();

    // Test each field
    expect(form.controls['currentPassword'].valid).toBeFalsy();
    expect(form.controls['newPassword'].valid).toBeFalsy();
    expect(form.controls['confirmPassword'].valid).toBeFalsy();
  });

  it('should validate password requirements', () => {
    authService.isAuthenticated = true;
    fixture.detectChanges();

    const form = component.changePasswordForm;
    form.controls['currentPassword'].setValue('test123');
    form.controls['newPassword'].setValue('short');
    form.controls['confirmPassword'].setValue('short');

    expect(form.valid).toBeFalsy();
    expect(form.controls['newPassword'].hasError('minlength')).toBeTruthy();
  });

  it('should validate password match', () => {
    authService.isAuthenticated = true;
    fixture.detectChanges();

    const form = component.changePasswordForm;
    form.controls['currentPassword'].setValue('test123');
    form.controls['newPassword'].setValue('ValidPass123!');
    form.controls['confirmPassword'].setValue('DifferentPass123!');

    expect(form.valid).toBeFalsy();
    expect(form.errors?.['passwordMismatch']).toBeTruthy();
  });

  it('should successfully submit valid form', fakeAsync(() => {
    // Set up spies
    authService.changePassword.and.returnValue(of({ message: 'Password changed successfully' }));
    authService.logout.and.returnValue(of(undefined));

    // Set authenticated and create component
    authService.isAuthenticated = true;
    fixture.detectChanges();

    // Wait for component to initialize
    fixture.detectChanges();

    // Set form values with valid password that meets all requirements
    component.changePasswordForm.patchValue({
      currentPassword: 'CurrentPass123!',
      newPassword: 'NewValidPass123!',
      confirmPassword: 'NewValidPass123!'
    });

    // Verify form is valid before submitting
    expect(component.changePasswordForm.valid).toBe(true);

    component.onSubmit();

    // Wait for the timeout in the component
    tick(2000);

    expect(authService.changePassword).toHaveBeenCalled();
    expect(authService.logout).toHaveBeenCalled();
  }));

  it('should handle error submission', () => {
    // Set up spy
    authService.changePassword.and.returnValue(throwError(() => ({ error: { message: 'Current password is incorrect' } })));

    // Set authenticated and create component
    authService.isAuthenticated = true;
    fixture.detectChanges();

    // Wait for component to initialize
    fixture.detectChanges();

    // Set form values with valid password that meets all requirements
    component.changePasswordForm.patchValue({
      currentPassword: 'CurrentPass123!',
      newPassword: 'NewValidPass123!',
      confirmPassword: 'NewValidPass123!'
    });

    // Verify form is valid before submitting
    expect(component.changePasswordForm.valid).toBe(true);

    component.onSubmit();

    expect(authService.changePassword).toHaveBeenCalled();
  });

  it('should toggle password visibility', () => {
    expect(component.passwordVisible.current).toBe(false);

    component.togglePasswordVisibility('current');
    expect(component.passwordVisible.current).toBe(true);

    component.togglePasswordVisibility('current');
    expect(component.passwordVisible.current).toBe(false);
  });

  it('should calculate password strength correctly', () => {
    // Test weak password
    component.changePasswordForm.controls['newPassword'].setValue('weak');
    expect(component.getPasswordStrengthClass()).toBe('weak');

    // Test medium password
    component.changePasswordForm.controls['newPassword'].setValue('Medium123');
    expect(component.getPasswordStrengthClass()).toBe('medium');

    // Test strong password
    component.changePasswordForm.controls['newPassword'].setValue('StrongPass123!');
    expect(component.getPasswordStrengthClass()).toBe('strong');
  });
});