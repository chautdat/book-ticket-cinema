import { AccountService } from '../services/account.service';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  AfterViewInit,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { Account, AccountLogin } from '../models/account.model';
import { AuthService } from '../services/auth.service';
import { jwtDecode } from 'jwt-decode';

declare let google: any;

interface DecodedToken {
  sub: string;
  email: string;
  name: string;
}

@Component({
  template: `
<section class="container-form forms">
    <div class="form login">
        <div class="form-content">
            <header>{{ 'login' | translate }}</header>
            <form [formGroup]="loginForm" (ngSubmit)="login()">
                <div class="field input-field">
                    <input type="email" formControlName="emailLogin" placeholder="Email" class="input">
                </div>

                <div class="field input-field">
                    <input type="password" formControlName="passwordLogin" placeholder="{{ 'password' | translate }}" class="password">
                    <i class='bx bx-hide eye-icon' (click)="togglePasswordVisibility($event)"></i>
                </div>

                <div class="field button-field">
                    <button>{{ 'login' | translate }}</button>
                </div>
            </form>

            <div class="form-link">
                <span>{{ 'not_account' | translate }}<a [routerLink]="['/login']" class="link signup-link"  (click)="toggleSignUp($event)">{{ 'register_now' | translate }}</a></span>
            </div>
        </div>

        <div class="line"></div>

        <!-- <div class="media-options">
            <a href="#" class="field google">
                <img src="/assets/img/google.png" alt="" class="google-img">
                <span>Login with Google</span>
            </a>
        </div> -->

        <div id="custom-google-button-container" class="custom-google-button">
            <div id="google-login-button" class="flex-row justify-center items-center hidden"></div>
        </div>

    </div>

    <!-- Signup Form -->

    <div class="form signup">
        <div class="form-content">
            <header>{{ 'signup' | translate }}</header>
            <form [formGroup]="signupForm" (ngSubmit)="signUp()" method="post">
                <!-- Username Field -->
                <div class="field input-field">
                    <input type="text" formControlName="username" placeholder="{{ 'username' | translate }}" class="input">
                </div>
                <div *ngIf="signupForm.get('username')?.invalid && signupForm.get('username')?.touched">
                    <small *ngIf="signupForm.get('username')?.errors?.['required']">{{ 'required_username' | translate }}</small>
                </div>
    
                <!-- Email Field -->
                <div class="field input-field">
                    <input type="email" formControlName="email" placeholder="Email" class="input">
                </div>
                <div *ngIf="signupForm.get('email')?.invalid && signupForm.get('email')?.touched">
                    <small *ngIf="signupForm.get('email')?.errors?.['required']">{{ 'required_email' | translate }}</small>
                </div>
    
                <!-- Phone Number Field -->
                <div class="field input-field">
                    <input type="tel" formControlName="phone" placeholder="{{ 'phone' | translate }}" class="input">
                </div>
                <div *ngIf="signupForm.get('phone')?.invalid && signupForm.get('phone')?.touched">
                    <small *ngIf="signupForm.get('phone')?.errors?.['required']">{{ 'required_phone' | translate }}</small>
                </div>
    
                <!-- Gender Field -->
                <div class="field input-field">
                    <select id="genderSelect" formControlName="gender" class="form-select">
                        <option value="" disabled selected>{{ 'gender' | translate }}</option>
                        <option value="{{ 'male' | translate }}">{{ 'male' | translate }}</option>
                        <option value="{{ 'female' | translate }}">{{ 'female' | translate }}</option>
                        <option value="{{ 'other' | translate }}">{{ 'other' | translate }}</option>
                    </select>
                </div>
                <div *ngIf="signupForm.get('gender')?.invalid && signupForm.get('gender')?.touched">
                    <small *ngIf="signupForm.get('gender')?.errors?.['required']">{{ 'required_gender' | translate }}</small>
                </div>
                
    
                <!-- Birthday Field with Date Picker -->
                <div class="field input-field">
                    <input type="text" formControlName="birthday" class="form-control" placeholder="{{ 'birth' | translate }}" bsDatepicker>
                </div>
                <div *ngIf="signupForm.get('birthday')?.invalid && signupForm.get('birthday')?.touched">
                    <small *ngIf="signupForm.get('birthday')?.errors?.['required']">{{ 'required_birth' | translate }}</small>
                </div>
    
                <!-- Password Field -->
                <div class="field input-field">
                    <input type="password"  formControlName="password" placeholder="{{ 'create_pass' | translate }}" class="password">
                    <i class='bx bx-hide eye-icon' (click)="togglePasswordVisibility($event)"></i>
                </div>
    
                <!-- Confirm Password Field -->
                <div class="field input-field">
                    <input type="password" pTooltip="{{ 'notice_pass' | translate }}" formControlName="confirmPassword" placeholder="{{ 'confirm_pass' | translate }}" class="password">
                    <i class='bx bx-hide eye-icon' (click)="togglePasswordVisibility($event)"></i>
                </div>
               
                <!-- Signup Button -->
                <div class="field button-field">
                    <button type="submit">{{'dang_ky' | translate }}</button>
                </div>
            </form>
    
            <div class="form-link">
                <span>{{'if' | translate }} <a [routerLink]="['/login']" class="link login-link" (click)="toggleSignUp($event)">{{'login' | translate }}</a></span>
            </div>
        </div>
    
        <div class="line"></div>
    
        <!-- <div class="media-options">
            <a href="#" class="field google">
                <img src="assets/img/google.png" alt="" class="google-img">
                <span>Login with Google</span>
            </a>
        </div> -->
        <div id="custom-google-button-container" class="custom-google-button">
            <div id="google-signin-button" pTooltip="mật khẩu mặc định là 123" class="flex-row justify-center items-center hidden"></div>
        </div>
    </div>
    
</section>
<p-toast></p-toast>
  `,
  styles: [
    `
/* Google Fonts - Poppins */
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap');


*{
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: 'Poppins', sans-serif;
}
.container-form{
    height: 65vh;
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    /* background-color: #4070f4; */
    column-gap: 30px;
}
.show-signup{
    height: 85vh;
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    /* background-color: #4070f4; */
    column-gap: 30px;
}
.form{
    position: absolute;
    max-width: 430px;
    width: 100%;
    padding: 30px;
    border-radius: 6px;
    background: #ebeaea;
}
.form.signup{
    opacity: 0;
    pointer-events: none;
}
.forms.show-signup .form.signup{
    opacity: 1;
    pointer-events: auto;
}
.forms.show-signup .form.login{
    opacity: 0;
    pointer-events: none;
}
header{
    font-size: 28px;
    font-weight: 600;
    color: #232836;
    text-align: center;
}
form{
    margin-top: 30px;
}
.form .field{
    position: relative;
    height: 50px;
    width: 100%;
    margin-top: 20px;
    border-radius: 6px;
}
.field input,
.field button{
    height: 100%;
    width: 100%;
    border: none;
    font-size: 16px;
    font-weight: 400;
    border-radius: 6px;
}
.field input{
    outline: none;
    padding: 0 15px;
    border: 1px solid#CACACA;
}
.field input:focus{
    border-bottom-width: 2px;
}
.eye-icon{
    position: absolute;
    top: 50%;
    right: 10px;
    transform: translateY(-50%);
    font-size: 18px;
    color: #8b8b8b;
    cursor: pointer;
    padding: 5px;
}
.field button{
    color: #fff;
    background-color: #0171d3;
    transition: all 0.3s ease;
    cursor: pointer;
}
.field button:hover{
    background-color: #016dcb;
}
.form-link{
    text-align: center;
    margin-top: 10px;
}
.form-link span,
.form-link a{
    font-size: 14px;
    font-weight: 400;
    color: #232836;
}
.form a{
    color: #0171d3;
    text-decoration: none;
}
.form-content a:hover{
    text-decoration: underline;
}
.line{
    position: relative;
    height: 1px;
    width: 100%;
    margin: 36px 0;
    background-color: #d4d4d4;
}
.line::before{
    content: 'Or';
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background-color: #ebeaea;
    color: #8b8b8b;
    padding: 0 15px;
}
.media-options a{
    display: flex;
    align-items: center;
    justify-content: center;
}
a.facebook{
    color: #fff;
    background-color: #4267b2;
}
a.facebook .facebook-icon{
    height: 28px;
    width: 28px;
    color: #0171d3;
    font-size: 20px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #fff;
}
.facebook-icon,
img.google-img{
    position: absolute;
    top: 50%;
    left: 15px;
    transform: translateY(-50%);
}
img.google-img{
    height: 20px;
    width: 20px;
    object-fit: cover;
}
a.google{
    border: 1px solid #CACACA;
}
a.google span{
    font-weight: 500;
    opacity: 0.6;
    color: #232836;
}

/* Ensure field container styles */
.field.input-field {
    margin-bottom: 15px;
}

/* Bootstrap form-select adjustments */
.form-select {
    border-radius: 4px;
    border: 1px solid #ced4da;
    padding: 10px;
    font-size: 16px;
    background-color: #ffffff;
}

/* On focus, change border color */
.form-select:focus {
    border-color: #007bff;
    outline: none;
}

/* Add margin to label */
.form-label {
    margin-bottom: 5px;
    font-weight: 600;
}

.custom-google-button {
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 8px;
  }
  
  #google-signin-button div {
    font-size: 18px !important; /* Tăng kích thước font chữ */
    font-weight: bold !important; /* Đậm hơn nếu cần */
  }

  #google-login-button div {
    font-size: 18px !important; /* Tăng kích thước font chữ */
    font-weight: bold !important; /* Đậm hơn nếu cần */
  }
  

@media screen and (max-width: 400px) {
    .form{
        padding: 20px 10px;
    }
    
}
    `
  ],
})
export class Login_SignupComponent implements OnInit, AfterViewInit {
  signupForm: FormGroup;
  loginForm: FormGroup;
  randomNumber = Math.floor(100000 + Math.random() * 900000);
  newAccount: Account;
  account: Account;
  authenticatedAccount: boolean;

  constructor(
    private el: ElementRef,
    private formBuilder: FormBuilder,
    private accountService: AccountService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.signupForm = this.formBuilder.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      gender: ['', Validators.required],
      birthday: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
    });

    this.loginForm = this.formBuilder.group({
      emailLogin: ['', [Validators.required, Validators.email]],
      passwordLogin: ['', Validators.required],
    });
  }

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.loadGoogleSignIn();
  }

  async signUp(): Promise<void> {
    // Kiểm tra mật khẩu xác nhận
    if (
      this.signupForm.value.password !== this.signupForm.value.confirmPassword
    ) {
      this.messageService.add({
        severity: 'error',
        summary: 'Xác nhận lại mật khẩu',
        detail:
          'Mật khẩu xác nhận không trùng với mật khẩu bạn tạo. Vui lòng nhập lại',
        life: 2000, // 2 giây
      });
      return;
    }

    // Kiểm tra tính hợp lệ của form
    if (!this.signupForm.valid) {
      this.messageService.add({
        severity: 'error',
        summary: 'Đăng ký thất bại',
        detail: 'Vui lòng nhập đầy đủ thông tin',
        life: 2000, // 2 giây
      });
      return;
    }

    try {
      // Định dạng ngày sinh - Format YYYY-MM-DD cho C# DateTime
      const birthday = this.signupForm.value.birthday;
      console.log('Birthday input:', birthday);

      const formattedBirthday = moment(birthday, 'DD/MM/YYYY').format(
        'YYYY-MM-DD'
      );
      console.log('Formatted birthday:', formattedBirthday);

      // Khởi tạo đối tượng newAccount
      this.newAccount = {
        id: 0,
        username: this.signupForm.value.username,
        email: this.signupForm.value.email,
        password: this.signupForm.value.password,
        phone: this.signupForm.value.phone,
        gender: this.signupForm.value.gender,
        birthday: formattedBirthday,
        securitycode: this.randomNumber.toString(),
        verify: 0,
        role: 1,
        created: new Date().toISOString().split('T')[0],
      };

      console.log(
        'Sending account data:',
        JSON.stringify(this.newAccount, null, 2)
      );

      // Tạo tài khoản
      const createRes = await this.accountService.create(this.newAccount);
      console.log('Create response:', createRes);

      // Kiểm tra response
      if (
        !createRes ||
        createRes.Status === false ||
        createRes.status === false
      ) {
        this.messageService.add({
          severity: 'error',
          summary: 'Đăng ký thất bại',
          detail:
            createRes?.Message ||
            createRes?.message ||
            'Không thể tạo tài khoản. Vui lòng thử lại.',
          life: 2500, // 2.5 giây
        });
        return;
      }

      // Tìm account vừa tạo
      const accountRes = await this.accountService.findByEmail(
        this.newAccount.email
      );

      if (!accountRes) {
        this.messageService.add({
          severity: 'error',
          summary: 'Lỗi',
          detail: 'Không tìm thấy tài khoản vừa tạo',
          life: 2000, // 2 giây
        });
        return;
      }

      this.account = accountRes;
      localStorage.setItem('account', JSON.stringify(this.account));

      // Hiển thị thông báo thành công
      this.messageService.add({
        severity: 'success',
        summary: 'Đăng ký thành công',
        detail: 'Chào mừng bạn đến với DQP CINEMA!',
        life: 2000,
      });

      // Tự động chuyển về trang chủ sau 1 giây
      setTimeout(() => {
        this.router.navigate(['/home']);
      }, 1000);
    } catch (error: any) {
      console.error('Error during signup:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Đăng ký thất bại',
        detail:
          error?.error?.Message ||
          error?.error?.message ||
          error?.message ||
          'Đã có lỗi xảy ra. Vui lòng thử lại.',
        life: 2500, // 2.5 giây - lỗi quan trọng
      });
    }
  }

  async login(): Promise<void> {
    if (!this.loginForm.valid) {
      this.messageService.add({
        severity: 'error',
        summary: 'Đăng nhập thất bại',
        detail: 'Bạn vui lòng nhập đầy đủ thông tin',
        life: 2000, // 2 giây
      });
      return;
    }

    const loginAccount: AccountLogin = {
      email: this.loginForm.value.emailLogin,
      password: this.loginForm.value.passwordLogin,
    };

    try {
      const res = await this.accountService.login(loginAccount);

      if (res.status) {
        this.account = res.data;
        console.log(this.account);

        // BỎ verify check - mọi account đã tự động verify khi đăng ký
        // if (this.account.verify !== 1) { ... }

        this.accountService.setAccount(this.account);
        this.messageService.add({
          severity: 'success',
          summary: 'Đăng nhập thành công',
          detail: 'Chào mừng bạn quay trở lại!',
          life: 1500,
        });
        this.router.navigate(['/home']);
      } else {
        this.messageService.add({
          severity: 'error',
          summary: 'Đăng nhập thất bại',
          detail: 'Email hoặc mật khẩu không đúng',
          life: 2500,
        });
      }
    } catch (error) {
      console.error('Login error:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Đăng nhập thất bại',
        detail: 'Đã có lỗi xảy ra. Vui lòng thử lại.',
        life: 2500,
      });
    }
  }

  toggleSignUp(event: Event): void {
    event.preventDefault();
    const forms = this.el.nativeElement.querySelector('.forms');
    forms.classList.toggle('show-signup');
  }

  loadGoogleSignIn(): void {
    if (typeof google !== 'undefined') {
      google.accounts.id.initialize({
        client_id:
          '431046856348-tqmndkgqdfuudqvp3n0j8ltvdqoqmtr2.apps.googleusercontent.com',
        callback: this.handleGoogleResponse.bind(this),
      });

      google.accounts.id.renderButton(
        document.getElementById('googleSignInButton'),
        { theme: 'outline', size: 'large', width: 370 }
      );
    }
  }

  handleGoogleResponse(response: any): void {
    if (response.credential) {
      const idToken = response.credential;

      try {
        const decodedToken: DecodedToken = jwtDecode(idToken);
        console.log('Decoded token:', decodedToken);

        this.authService.googleRegister(idToken).subscribe(
          (res: any) => {
            if (res && res.status) {
              this.account = res.data;
              this.accountService.setAccount(this.account);

              this.messageService.add({
                severity: 'success',
                summary: 'Đăng nhập thành công',
                detail: 'Chào mừng bạn đến với DQP CINEMA!',
              });

              this.router.navigate(['/home']);
            } else {
              this.messageService.add({
                severity: 'error',
                summary: 'Đăng nhập thất bại',
                detail: res?.message || 'Không thể đăng nhập bằng Google',
              });
            }
          },
          (error) => {
            console.error('Google login error:', error);
            this.messageService.add({
              severity: 'error',
              summary: 'Đăng nhập thất bại',
              detail: 'Đã có lỗi xảy ra khi đăng nhập bằng Google',
            });
          }
        );
      } catch (error) {
        console.error('Error decoding token:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Lỗi',
          detail: 'Không thể xác thực với Google',
        });
      }
    }
  }

  togglePasswordVisibility(event: Event): void {
    event.preventDefault();
    const icon = event.target as HTMLElement;
    const input = icon.previousElementSibling as HTMLInputElement;

    if (input) {
      if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('bx-hide');
        icon.classList.add('bx-show');
      } else {
        input.type = 'password';
        icon.classList.remove('bx-show');
        icon.classList.add('bx-hide');
      }
    }
  }
}
