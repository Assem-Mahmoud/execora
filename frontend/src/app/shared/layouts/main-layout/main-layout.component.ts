import { Component, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { TenantRole } from '../../core/models';

/**
 * Main layout component for authenticated pages
 * Includes sidebar navigation, header with user menu, and main content area
 */
@Component({
  selector: 'exe-main-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss']
})
export class MainLayoutComponent {
  /** Show/hide sidebar */
  readonly sidebarCollapsed = input<boolean>(false);

  /** Sidebar collapse toggle output */
  readonly sidebarToggle = output<void>();

  /** Navigation items for sidebar */
  readonly navItems = [
    {
      path: '/app/dashboard',
      icon: 'home',
      label: 'Dashboard',
      roles: [TenantRole.ProjectManager, TenantRole.ProjectAdmin, TenantRole.TenantAdmin]
    },
    {
      path: '/app/projects',
      icon: 'folder',
      label: 'Projects',
      roles: [TenantRole.SiteEngineer, TenantRole.ProjectManager, TenantRole.ProjectAdmin, TenantRole.TenantAdmin]
    },
    {
      path: '/app/activities',
      icon: 'list-check',
      label: 'Activities',
      roles: [TenantRole.SiteEngineer, TenantRole.ProjectManager, TenantRole.QAQC]
    },
    {
      path: '/app/inspections',
      icon: 'clipboard-check',
      label: 'Inspections',
      roles: [TenantRole.QAQC, TenantRole.ProjectManager]
    },
    {
      path: '/app/issues',
      icon: 'alert-triangle',
      label: 'Issues',
      roles: [TenantRole.SiteEngineer, TenantRole.ProjectManager, TenantRole.ProjectAdmin]
    },
    {
      path: '/app/ncr',
      icon: 'file-text',
      label: 'NCRs',
      roles: [TenantRole.QAQC, TenantRole.ProjectManager]
    },
    {
      path: '/app/daily-ops',
      icon: 'calendar',
      label: 'Daily Reports',
      roles: [TenantRole.SiteEngineer]
    },
    {
      path: '/app/bim',
      icon: 'box',
      label: 'BIM Viewer',
      roles: [TenantRole.ProjectManager, TenantRole.TenantAdmin]
    },
    {
      path: '/app/dashboards',
      icon: 'chart-bar',
      label: 'Dashboards',
      roles: [TenantRole.ProjectAdmin, TenantRole.TenantAdmin]
    },
    {
      path: '/app/settings',
      icon: 'settings',
      label: 'Settings',
      roles: [TenantRole.TenantAdmin]
    }
  ];

  /** User menu items */
  readonly userMenuItems = [
    { path: '/app/profile', icon: 'user', label: 'My Profile' },
    { path: '/app/notifications', icon: 'bell', label: 'Notifications' },
    { path: '/auth/logout', icon: 'log-out', label: 'Logout' }
  ];

  constructor(protected authService: AuthService) {}

  /**
   * Get filtered navigation items based on user's role
   */
  getFilteredNavItems() {
    const userRole = this.authService.currentRole;
    if (!userRole) {
      return [];
    }

    return this.navItems.filter(item =>
      item.roles.some(role => this.hasSufficientRole(userRole, role))
    );
  }

  /**
   * Check if user's role has sufficient privileges
   */
  private hasSufficientRole(userRole: TenantRole, requiredRole: TenantRole): boolean {
    const roleHierarchy: Record<TenantRole, number> = {
      [TenantRole.SystemAdmin]: 1,
      [TenantRole.TenantAdmin]: 2,
      [TenantRole.ProjectAdmin]: 3,
      [TenantRole.ProjectManager]: 4,
      [TenantRole.QAQC]: 5,
      [TenantRole.SiteEngineer]: 6
    };

    const userLevel = roleHierarchy[userRole] ?? Number.MAX_VALUE;
    const requiredLevel = roleHierarchy[requiredRole] ?? Number.MAX_VALUE;

    return userLevel <= requiredLevel;
  }

  /**
   * Handle sidebar toggle
   */
  toggleSidebar(): void {
    this.sidebarToggle.emit();
  }

  /**
   * Get user initials for avatar
   */
  getUserInitials(): string {
    const user = this.authService.currentUser;
    if (!user) {
      return '?';
    }
    return `${user.firstName[0]}${user.lastName[0]}`.toUpperCase();
  }

  /**
   * Get user's display name
   */
  getUserDisplayName(): string {
    const user = this.authService.currentUser;
    if (!user) {
      return '';
    }
    return `${user.firstName} ${user.lastName}`;
  }

  /**
   * Get current tenant name
   */
  getTenantName(): string {
    const tenant = this.authService.currentTenant;
    return tenant?.name || '';
  }
}
