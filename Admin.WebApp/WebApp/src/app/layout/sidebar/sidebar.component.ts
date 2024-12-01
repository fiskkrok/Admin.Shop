import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService } from '../../core/services/theme.service';
import { MatSidenavModule } from '@angular/material/sidenav';
import { AuthService } from 'src/app/core/services/auth.service';

interface NavItem {
  path: string;
  icon: string;
  label: string;
  children?: NavItem[];
}
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatSidenavModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})

export class SidebarComponent {
  isCollapsed = false;
  isDarkTheme = true;
  logo = '../../../assets/dashboardadmin.png';
  openSubmenus = new Set<string>();
  currentUser = { name: '' };
  isUserMenuOpen = false;

  navItems: NavItem[] = [
    {
      path: '/products',
      icon: 'bi-box',
      label: 'Products',
      children: [
        {
          path: '/products/list',
          icon: 'bi-list-ul',
          label: 'List Products'
        },
        {
          path: '/products/add',
          icon: 'bi-plus-circle',
          label: 'Add Product'
        },
        {
          path: '/products/remove',
          icon: 'bi-trash',
          label: 'Remove Products'
        }
      ]
    },
    {
      path: '/statistics',
      icon: 'bi-graph-up',
      label: 'Statistics'
    }
  ];

  // Flattened navigation for mobile
  get mobileNavItems(): NavItem[] {
    return this.navItems.reduce((acc: NavItem[], item) => {
      if (item.children) {
        return [...acc, ...item.children];
      }
      return [...acc, item];
    }, []);
  }

  constructor(
    private readonly themeService: ThemeService,
    private readonly authService: AuthService
  ) {
    this.isDarkTheme = this.themeService.isDarkTheme();
    this.currentUser.name = this.authService.getCurrentUserName();
  }

  toggleSidebar() {
    this.isCollapsed = !this.isCollapsed;
    if (this.isCollapsed) {
      this.openSubmenus.clear();
    }
  }

  toggleTheme() {
    this.isDarkTheme = !this.isDarkTheme;
    // this.themeService.setTheme(this.isDarkTheme ? 'dark' : 'light');
    this.themeService.setTheme(this.isDarkTheme);

  }

  toggleSubmenu(item: NavItem) {
    if (this.isSubmenuOpen(item)) {
      this.openSubmenus.delete(item.path);
    } else {
      this.openSubmenus.add(item.path);
    }
  }

  isSubmenuOpen(item: NavItem): boolean {
    return this.openSubmenus.has(item.path);
  }

  isChildActive(item: NavItem): boolean {
    return item.children?.some(child =>
      window.location.pathname.startsWith(child.path)
    ) ?? false;
  }

  toggleUserMenu() {
    this.isUserMenuOpen = !this.isUserMenuOpen;
  }

  logout() {
    this.authService.logout();
  }
}