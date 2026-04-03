import { Injectable } from '@angular/core';
import { CanActivate, Router } from "@angular/router";
import { AccountService } from './account.service';
import { Account } from '../models/account.model';
import { MessageService } from 'primeng/api';
@Injectable()
export class CheckLoginService implements CanActivate{
    constructor(
        private accountService: AccountService,
        private router: Router,
        private messageService: MessageService
    ){}
    account: Account;
    canActivate(){
        this.account =  JSON.parse(localStorage.getItem("account"));
        console.log(this.account);
        if(this.account != null){
            return true;
        } else{
            this.messageService.add({
                severity: "error",
                summary: "Thông báo",
                detail: "Mời bạn đăng nhập"
              });
           return false;
        }
     
    }
}