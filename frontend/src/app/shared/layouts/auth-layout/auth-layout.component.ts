import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/**
 * Authentication layout for login/register pages
 * Clean, centered layout without navigation
 */
@Component({
  selector: 'exe-auth-layout',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './auth-layout.component.html',
  styleUrls: ['./auth-layout.component.scss']
})
export class AuthLayoutComponent {}
