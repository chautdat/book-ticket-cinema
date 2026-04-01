import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
<p-toast position="top-right"></p-toast>
<p-confirmDialog></p-confirmDialog>
<router-outlet></router-outlet>

  `,
  styles: [
    `

    `
  ],})
export class AppComponent {
  title = 'cinema_admin';
}
