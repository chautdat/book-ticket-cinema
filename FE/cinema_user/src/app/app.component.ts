import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

import { MovieService } from './services/movie.service';
import { Account } from './models/account.model';
import { Movie } from './models/movie.model';
import { AccountService } from './services/account.service';
import { MessageService } from 'primeng/api';
import { FollowService } from './services/follow.service';
import { TranslateService } from '@ngx-translate/core';

interface AutoCompleteCompleteEvent {
  originalEvent: Event;
  query: string;
}

@Component({
  selector: 'app-root',
  template: `
<!-- Main Container -->
<div class="cinema-app">
  <!-- Topbar -->
  <div class="topbar">
    <div class="topbar-content">
      <!-- Left Side -->
      <div class="topbar-left">
        <span class="info-item">
          <i class="fas fa-phone-alt"></i>
          {{ "hotline" | translate }}
        </span>
        <span class="topbar-divider">|</span>
        <span class="info-item">
          <i class="fas fa-envelope"></i>
          {{ "email" | translate }}
        </span>
      </div>

      <!-- Right Side -->
      <div class="topbar-right">
        <ng-container *ngIf="account">
          <a [routerLink]="['/profile']" class="topbar-link">
            <i class="fas fa-user-circle"></i>
            {{ "tai_khoan" | translate }}
          </a>
          <span class="topbar-divider">|</span>
          <a [routerLink]="['/home']" (click)="clearData()" class="topbar-link">
            <i class="fas fa-sign-out-alt"></i>
            {{ "dang_xuat" | translate }}
          </a>
        </ng-container>
        <ng-container *ngIf="!account">
          <a [routerLink]="['/login']" class="topbar-link">
            <i class="fas fa-sign-in-alt"></i>
            {{ "dang_nhap" | translate }}
          </a>
          <span class="topbar-divider">|</span>
          <a [routerLink]="['/login']" class="topbar-link highlight">
            <i class="fas fa-user-plus"></i>
            {{ "dang_ky" | translate }}
          </a>
        </ng-container>
      </div>
    </div>
  </div>

  <!-- Navbar -->
  <nav class="navbar navbar-expand-lg">
    <div class="navbar-content">
      <!-- Left: Logo -->
      <a [routerLink]="['/home']" class="logo">
        <div class="logo-icon">
          <img src="assets/img/aaaaaa.png" alt="Logo" />
        </div>
        <div class="logo-text">
          <span class="logo-main">DQP CINEMA</span>
          <span class="logo-tagline">Experience The Magic</span>
        </div>
      </a>

      <!-- Mobile Toggle Button -->
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarMain"
      >
        <i class="fas fa-bars"></i>
      </button>

      <!-- Collapsible Content -->
      <div class="collapse navbar-collapse" id="navbarMain">
        <!-- Center: Navigation Links -->
        <div class="nav-center">
          <a
            [routerLink]="['/home']"
            routerLinkActive="active"
            [routerLinkActiveOptions]="{ exact: true }"
            class="nav-link"
          >
            <i class="fas fa-home"></i>
            <span>{{ "home" | translate }}</span>
          </a>
          <a
            [routerLink]="['/news']"
            routerLinkActive="active"
            class="nav-link"
          >
            <i class="fas fa-film"></i>
            <span>{{ "now_showing" | translate }}</span>
          </a>
          <a
            [routerLink]="['/cinema']"
            routerLinkActive="active"
            class="nav-link"
          >
            <i class="fas fa-clock"></i>
            <span>{{ "showtime" | translate }}</span>
          </a>
          <a
            [routerLink]="['/pricing']"
            routerLinkActive="active"
            class="nav-link"
          >
            <i class="fas fa-info-circle"></i>
            <span>{{ "information" | translate }}</span>
          </a>
          <a
            *ngIf="account"
            [routerLink]="['/tickets/history']"
            routerLinkActive="active"
            class="nav-link"
          >
            <i class="fas fa-ticket-alt"></i>
            <span>Lịch sử vé</span>
          </a>
        </div>

        <!-- Right: Search + CTA -->
        <div class="nav-right">
          <div class="search-wrapper">
            <i class="fas fa-search"></i>
            <p-autoComplete
              [(ngModel)]="movie"
              [suggestions]="filteredMovies"
              (completeMethod)="filterMovies($event)"
              field="title"
              placeholder="{{ 'search_placeholder' | translate }}"
              (onSelect)="onMovieSelect($event)"
            />
          </div>
        </div>
      </div>
    </div>
  </nav>

  <!-- Main Content -->
  <main class="main-content">
    <router-outlet></router-outlet>
  </main>

  <!-- Footer -->
  <footer class="footer">
    <div class="footer-main">
      <div class="footer-container">
        <div class="footer-grid">
          <!-- Brand -->
          <div class="footer-brand">
            <div class="brand-logo">
              <img src="assets/img/aaaaaa.png" alt="Logo" />
              <h3>DQP CINEMA</h3>
            </div>
            <p class="brand-desc">{{ "company_name" | translate }}</p>
            <div class="social-links">
              <a href="https://github.com/" class="social-btn"
                ><i class="fab fa-github"></i
              ></a>
              <a href="https://www.facebook.com/" class="social-btn"
                ><i class="fab fa-facebook-f"></i
              ></a>
              <a href="https://www.youtube.com/" class="social-btn"
                ><i class="fab fa-youtube"></i
              ></a>
              <a href="https://www.instagram.com/" class="social-btn"
                ><i class="fab fa-instagram"></i
              ></a>
            </div>
          </div>

          <!-- Contact -->
          <div class="footer-col">
            <h4>{{ "footer_location" | translate }}</h4>
            <ul class="contact-list">
              <li>
                <i class="fas fa-map-marker-alt"></i
                ><span>{{ "dia_chi" | translate }}</span>
              </li>
              <li>
                <i class="fas fa-phone-alt"></i
                ><span>{{ "hotline" | translate }}</span>
              </li>
              <li>
                <i class="fas fa-envelope"></i
                ><span>{{ "email" | translate }}</span>
              </li>
            </ul>
          </div>

          <!-- Hours -->
          <div class="footer-col">
            <h4>{{ "footer_hours" | translate }}</h4>
            <div class="hours-block">
              <div class="hour-item">
                <span class="label">{{ "weekday_hours" | translate }}</span>
                <span class="value">{{ "weekday_times" | translate }}</span>
              </div>
              <div class="hour-item">
                <span class="label">{{ "sunday_hours" | translate }}</span>
                <span class="value">{{ "sunday_times" | translate }}</span>
              </div>
            </div>
          </div>

          <!-- Subscribe -->
          <div class="footer-col">
            <h4>{{ "ready_to_connect" | translate }}</h4>
            <p class="subscribe-text">
              Theo dõi để nhận thông báo phim mới nhất!
            </p>
            <button class="subscribe-btn" (click)="follow()">
              <i
                class="fas"
                [ngClass]="isFollowed ? 'fa-bell-slash' : 'fa-bell'"
              ></i>
              {{ isFollowed ? "Bỏ theo dõi" : "Theo dõi ngay" }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer Bottom -->
    <div class="footer-bottom">
      <div class="footer-bottom-inner">
        <p>&copy; 2024 DQP Cinema. All rights reserved.</p>
        <div class="footer-bottom-links">
          <a href="#">Điều khoản</a>
          <a href="#">Chính sách</a>
          <a href="#">Hỗ trợ</a>
        </div>
      </div>
    </div>
  </footer>

  <!-- Back to Top (CSS only) -->
  <a href="#" class="back-to-top">
    <i class="fas fa-arrow-up"></i>
  </a>

  <app-chat></app-chat>
</div>

<p-toast></p-toast>

  `,
  styles: [
    `
/* ============================================
   DQP CINEMA - White Theme Layout
   Topbar đen | Navbar trắng | Background trắng
   ============================================ */

/* CSS Variables */
:host {
  --primary: #e50914;
  --primary-hover: #f40612;
  --primary-dark: #b20710;
  --black: #1a1a1a;
  --black-soft: #222222;
  --gray-900: #333333;
  --gray-800: #444444;
  --gray-700: #555555;
  --gray-600: #666666;
  --gray-500: #888888;
  --gray-400: #999999;
  --gray-300: #bbbbbb;
  --gray-200: #e0e0e0;
  --gray-100: #f5f5f5;
  --white: #ffffff;
  --gold: #d4af37;

  --font-display: "Bebas Neue", "Impact", sans-serif;
  --font-body: "Poppins", -apple-system, BlinkMacSystemFont, sans-serif;

  --shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  --shadow-md: 0 4px 20px rgba(0, 0, 0, 0.15);
  --shadow-lg: 0 8px 30px rgba(0, 0, 0, 0.2);
  --glow: 0 0 20px rgba(229, 9, 20, 0.3);

  --radius-sm: 6px;
  --radius-md: 10px;
  --radius-lg: 50px;

  --transition: all 0.3s ease;
}

/* Base - Background trắng */
.cinema-app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: var(--white);
  color: var(--gray-900);
  font-family: var(--font-body);
}

/* ============================================
   TOPBAR - Nền đen
   ============================================ */
.topbar {
  background: var(--black);
  border-bottom: 1px solid var(--gray-800);
}

.topbar-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0.6rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.topbar-left,
.topbar-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--gray-400);
  font-size: 0.8rem;
}

.info-item i {
  color: var(--primary);
  font-size: 0.75rem;
}

.topbar-divider {
  color: var(--gray-700);
  font-size: 0.75rem;
}

.topbar-link {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  color: var(--gray-300);
  text-decoration: none;
  font-size: 0.8rem;
  transition: var(--transition);
}

.topbar-link:hover {
  color: var(--primary);
}

.topbar-link.highlight {
  color: var(--primary);
  font-weight: 500;
}

/* ============================================
   NAVBAR - Nền trắng
   ============================================ */
.navbar {
  background: var(--white);
  position: sticky;
  top: 0;
  z-index: 1000;
  box-shadow: var(--shadow-md);
  padding: 0 !important;
}

.navbar-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 2rem;
  min-height: 80px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 2rem;
  flex-wrap: wrap;
}

/* Logo */
.logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  flex-shrink: 0;
}

.logo-icon img {
  width: 50px;
  height: 46px;
  object-fit: contain;
  transition: var(--transition);
}

.logo:hover .logo-icon img {
  transform: scale(1.05);
}

.logo-text {
  display: flex;
  flex-direction: column;
}

.logo-main {
  font-family: var(--font-display);
  font-size: 1.75rem;
  color: var(--primary);
  letter-spacing: 0.08em;
  line-height: 1;
  font-weight: 700;
}

.logo-tagline {
  font-size: 0.6rem;
  color: var(--gray-600);
  letter-spacing: 0.2em;
  text-transform: uppercase;
  margin-top: 2px;
}

/* Navbar Toggler */
.navbar-toggler {
  background: var(--gray-100);
  border: 1px solid var(--gray-200);
  color: var(--gray-900);
  padding: 0.5rem 0.75rem;
  border-radius: var(--radius-sm);
  font-size: 1.25rem;
  transition: var(--transition);
}

.navbar-toggler:hover {
  background: var(--primary);
  border-color: var(--primary);
  color: var(--white);
}

.navbar-toggler:focus {
  box-shadow: none;
}

/* Navbar Collapse */
.navbar-collapse {
  flex-grow: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

/* Nav Center - Menu giữa */
.nav-center {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin: 0 auto;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  color: var(--gray-700) !important;
  text-decoration: none;
  font-size: 0.9rem;
  font-weight: 500;
  border-radius: var(--radius-md);
  transition: var(--transition);
  position: relative;
}

.nav-link i {
  font-size: 0.85rem;
  transition: var(--transition);
}

.nav-link::after {
  content: "";
  position: absolute;
  bottom: 4px;
  left: 50%;
  transform: translateX(-50%);
  width: 0;
  height: 2px;
  background: var(--primary);
  transition: var(--transition);
  border-radius: 2px;
}

.nav-link:hover {
  color: var(--primary) !important;
  background: var(--gray-100);
}

.nav-link:hover::after {
  width: 50%;
}

.nav-link:hover i {
  color: var(--primary);
}

.nav-link.active {
  color: var(--primary) !important;
}

.nav-link.active::after {
  width: 50%;
}

.nav-link.active i {
  color: var(--primary);
}

/* Nav Right - Search + CTA */
.nav-right {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-shrink: 0;
}

/* Search Box */
.search-wrapper {
  position: relative;
  width: 200px;
}

.search-wrapper > i {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--gray-500);
  font-size: 0.85rem;
  z-index: 1;
  pointer-events: none;
}

.search-wrapper ::ng-deep .p-autocomplete {
  width: 100%;
}

.search-wrapper ::ng-deep .p-autocomplete-input {
  width: 100%;
  background: var(--gray-100);
  border: 1px solid var(--gray-200);
  border-radius: var(--radius-lg);
  padding: 0.65rem 1rem 0.65rem 2.5rem;
  color: var(--gray-900);
  font-size: 0.85rem;
  transition: var(--transition);
}

.search-wrapper ::ng-deep .p-autocomplete-input:focus {
  border-color: var(--primary);
  box-shadow: 0 0 0 3px rgba(229, 9, 20, 0.1);
  outline: none;
  background: var(--white);
}

.search-wrapper ::ng-deep .p-autocomplete-input::placeholder {
  color: var(--gray-500);
}

/* CTA Button - Nút MUA VÉ */
.cta-btn {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  padding: 0.75rem 1.5rem;
  background: var(--primary);
  color: var(--white);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.85rem;
  border-radius: var(--radius-lg);
  transition: var(--transition);
  box-shadow: 0 4px 15px rgba(229, 9, 20, 0.3);
  text-transform: uppercase;
  letter-spacing: 0.03em;
  white-space: nowrap;
}

.cta-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 25px rgba(229, 9, 20, 0.4);
  background: var(--primary-hover);
  color: var(--white);
}

.cta-btn i {
  font-size: 0.9rem;
}

/* ============================================
   MAIN CONTENT
   ============================================ */
.main-content {
  flex: 1;
  background: var(--white);
}

/* ============================================
   FOOTER - Nền tối
   ============================================ */
.footer {
  background: var(--black);
  color: var(--white);
  margin-top: auto;
}

.footer-main {
  padding: 4rem 0;
  position: relative;
}

/* Film strip decoration */
/* Bỏ đường gạch trang trí trên đầu footer */
.footer-main::before {
  content: none;
}

.footer-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 2rem;
}

.footer-grid {
  display: grid;
  grid-template-columns: 1.5fr 1fr 1fr 1fr;
  gap: 3rem;
}

/* Footer Brand */
.footer-brand {
  padding-right: 2rem;
}

.brand-logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.brand-logo img {
  width: 45px;
  height: 42px;
  object-fit: contain;
}

.brand-logo h3 {
  font-family: var(--font-display);
  font-size: 1.5rem;
  color: var(--primary);
  letter-spacing: 0.08em;
  margin: 0;
}

.brand-desc {
  color: var(--gray-400);
  font-size: 0.9rem;
  line-height: 1.6;
  margin-bottom: 1.5rem;
}

.social-links {
  display: flex;
  gap: 0.75rem;
}

.social-btn {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--gray-800);
  color: var(--white);
  border-radius: 50%;
  text-decoration: none;
  transition: var(--transition);
}

.social-btn:hover {
  background: var(--primary);
  transform: translateY(-3px);
  box-shadow: var(--glow);
  color: var(--white);
}

/* Footer Columns */
.footer-col h4 {
  color: var(--white);
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 1.25rem;
  padding-bottom: 0.75rem;
  position: relative;
}

.footer-col h4::after {
  content: "";
  position: absolute;
  bottom: 0;
  left: 0;
  width: 35px;
  height: 2px;
  background: var(--primary);
}

.contact-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.contact-list li {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  margin-bottom: 0.9rem;
  color: var(--gray-400);
  font-size: 0.875rem;
  line-height: 1.5;
}

.contact-list li i {
  color: var(--primary);
  font-size: 0.8rem;
  margin-top: 3px;
  width: 16px;
  flex-shrink: 0;
}

/* Hours */
.hours-block {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.hour-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.hour-item .label {
  color: var(--white);
  font-size: 0.875rem;
  font-weight: 500;
}

.hour-item .value {
  color: var(--gray-400);
  font-size: 0.875rem;
}

/* Subscribe */
.subscribe-text {
  color: var(--gray-400);
  font-size: 0.875rem;
  line-height: 1.6;
  margin-bottom: 1rem;
}

.subscribe-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  background: transparent;
  border: 2px solid var(--primary);
  color: var(--primary);
  font-size: 0.85rem;
  font-weight: 600;
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: var(--transition);
}

.subscribe-btn:hover {
  background: var(--primary);
  color: var(--white);
  box-shadow: var(--glow);
}

/* Footer Bottom */
.footer-bottom {
  background: rgba(0, 0, 0, 0.3);
  border-top: 1px solid var(--gray-800);
}

.footer-bottom-inner {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1.25rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.footer-bottom p {
  color: var(--gray-500);
  font-size: 0.85rem;
  margin: 0;
}

.footer-bottom-links {
  display: flex;
  gap: 1.5rem;
}

.footer-bottom-links a {
  color: var(--gray-500);
  text-decoration: none;
  font-size: 0.85rem;
  transition: var(--transition);
}

.footer-bottom-links a:hover {
  color: var(--primary);
}

/* ============================================
   BACK TO TOP
   ============================================ */
.back-to-top {
  position: fixed;
  bottom: 6.25rem;
  right: 2rem;
  width: 50px;
  height: 50px;
  background: var(--primary);
  color: var(--white);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  text-decoration: none;
  opacity: 0.9;
  transition: var(--transition);
  box-shadow: var(--shadow-md);
  z-index: 999;
}

.back-to-top:hover {
  background: var(--primary-hover);
  transform: translateY(-3px);
  box-shadow: var(--glow);
  color: var(--white);
}

/* ============================================
   RESPONSIVE
   ============================================ */
@media (max-width: 1200px) {
  .nav-center {
    gap: 0.25rem;
  }

  .nav-link {
    padding: 0.6rem 1rem;
    font-size: 0.85rem;
  }

  .search-wrapper {
    width: 180px;
  }

  .footer-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 2.5rem;
  }
}

@media (max-width: 992px) {
  .navbar-content {
    min-height: 70px;
    padding: 0.5rem 1rem;
  }

  .navbar-collapse {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    background: var(--white);
    padding: 1.5rem;
    border-top: 1px solid var(--gray-200);
    box-shadow: var(--shadow-md);
    flex-direction: column;
    gap: 1.5rem;
  }

  .nav-center {
    flex-direction: column;
    width: 100%;
    gap: 0.5rem;
  }

  .nav-link {
    width: 100%;
    justify-content: center;
    padding: 1rem;
    background: var(--gray-100);
  }

  .nav-link::after {
    display: none;
  }

  .nav-right {
    flex-direction: column;
    width: 100%;
    gap: 1rem;
  }

  .search-wrapper {
    width: 100%;
  }

  .cta-btn {
    width: 100%;
    justify-content: center;
    padding: 1rem;
  }
}

@media (max-width: 768px) {
  .topbar-left {
    display: none;
  }

  .topbar-content {
    justify-content: center;
  }

  .topbar-right {
    gap: 0.75rem;
  }

  .logo-main {
    font-size: 1.4rem;
  }

  .logo-tagline {
    display: none;
  }

  .footer-grid {
    grid-template-columns: 1fr;
    gap: 2rem;
    text-align: center;
  }

  .footer-brand {
    padding-right: 0;
  }

  .brand-logo {
    justify-content: center;
  }

  .social-links {
    justify-content: center;
  }

  .footer-col h4::after {
    left: 50%;
    transform: translateX(-50%);
  }

  .contact-list li {
    justify-content: center;
  }

  .hours-block,
  .hour-item {
    align-items: center;
  }

  .subscribe-btn {
    width: 100%;
    justify-content: center;
  }

  .footer-bottom-inner {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .footer-container {
    padding: 0 1rem;
  }
}

@media (max-width: 480px) {
  .topbar-link span {
    display: none;
  }

  .topbar-link i {
    font-size: 1rem;
  }

  .logo-icon img {
    width: 40px;
    height: 37px;
  }

  .logo-main {
    font-size: 1.2rem;
  }

  .back-to-top {
    bottom: 5rem;
    right: 1rem;
    width: 44px;
    height: 44px;
  }
}

/* Remove any border/outline around footer area */
.footer,
footer {
  border: none !important;
  outline: none !important;
  box-shadow: none !important;
}

    `
  ],})
export class AppComponent implements OnInit {
  account: Account | null = null; 
  movies: Movie[] = []; 
  filteredMovies: Movie[] = []; 
  movie: Movie | null = null; 
  private routeSub: Subscription = new Subscription(); 
  isFollowed: boolean;
  constructor(
    private router: Router,
    private cdr: ChangeDetectorRef,
    private movieService: MovieService,
    private route: ActivatedRoute,
    private accountService: AccountService,
    private messageService: MessageService,
    private followService: FollowService,
    private translate: TranslateService
  ) {
    translate.setDefaultLang('vi');
    translate.use('vi');
  }

  ngOnInit(): void {
    const accountRaw = localStorage.getItem('account');
    if (accountRaw) {
      try {
        const account = JSON.parse(accountRaw);
        if (account?.id) {
          this.followService.findById(account.id).then(res => {
            this.isFollowed = !!res?.status;
          });
        }
      } catch (error) {
        console.warn('Invalid account data in localStorage', error);
      }
    }
   
    this.accountService.getAccount().subscribe(account => {
      if (account) {
          this.account = account;
      } else {
          const accountData = localStorage.getItem('account');
          if (accountData) {
              this.account = JSON.parse(accountData);
          }
      }
  });
    this.loadMovies();
  }

  private async loadMovies(): Promise<void> {
    try {
      const movies = await this.movieService.findAllByStatus();
      this.movies = movies as Movie[];
      console.log('Loaded Movies:', this.movies); // Check the data
    } catch (error) {
      console.error('Error loading movies:', error);
    }
  }

  private loadAccount(): void {
    const accountData = localStorage.getItem('account');
    this.account = accountData ? JSON.parse(accountData) : null;
  }

  getFromLocal(key: string): string | null {
    return localStorage.getItem(key);
  }

  async clearData(): Promise<void> {
    localStorage.removeItem('account');
    this.account = null;
    this.cdr.detectChanges();
    await this.router.navigate(['/home']);
  }

  // filterMovies(event: AutoCompleteCompleteEvent): void {
  //   const query = event.query.toLowerCase();
  //   this.filteredMovies = this.movies.filter(movie => 
  //     movie.title.toLowerCase().startsWith(query)
  //   );
  // }

  // onMovieSelect(event: any): void {
  //   if (this.movie) {
  //     this.router.navigate(['/movie-details', this.movie.id]).then(() => {
  //       window.location.reload();
  //       this.movie = null; // Clear selection
  //     });
     
  //   }
  // }
  filterMovies(event: AutoCompleteCompleteEvent): void {
    const query = event.query.toLowerCase();
    this.filteredMovies = this.movies.filter(movie => 
      movie.title.toLowerCase().includes(query)
    );
}

onMovieSelect(event: any): void {
    if (this.movie) {
      this.router.navigate(['/movie-details', this.movie.id]).then(() => {
        window.location.reload();
        this.movie = null; // Clear selection
      });
    }
}

  title = 'mall_admin';
  follow(){
  
    const account = JSON.parse(localStorage.getItem('account'));
   
    if(account == null){
      this.messageService.add({
        severity: "error",
        summary: "Vui lòng đăng nhập",
        detail: "Lỗi"
      });
    } else {
      if(this.isFollowed){
        this.followService.findById(account.id).then(
          res => {
            var follow = {
              accountId: account.id,
              status: false,
              id: res.id
            }
            this.followService.create(follow).then(
              res => {
                this.messageService.add({
                  severity: "success",
                  summary: "Bỏ theo dõi thành công",
                  detail: "Thành công"
                });
                this.isFollowed = false;
                console.log(res);
              }
            );
          }
        );
      } else {
        this.followService.findById(account.id).then(
          res => {
            if(res == null){
              var follow = {
                accountId: account.id,
                status: true
              }
              this.followService.create(follow).then(
                res => {
                  this.messageService.add({
                    severity: "success",
                    summary: "Theo dõi thành công",
                    detail: "Bạn sẽ nhận được thông báo phim hằng ngày."
                  });
                  this.isFollowed = true;
                  console.log(res);
                }
              );
            } else {
              var follow1 = {
                accountId: account.id,
                status: true,
                id: res.id
              }
              this.followService.create(follow1).then(
                res => {
                  this.messageService.add({
                    severity: "success",
                    summary: "Theo dõi thành công",
                    detail: "Bạn sẽ nhận được thông báo phim hằng ngày."
                  });
                  this.isFollowed = true;
                  console.log(res);
                }
              );
            }
          }
        );
       
      }
     
    }
    console.log(account);
  }
}
