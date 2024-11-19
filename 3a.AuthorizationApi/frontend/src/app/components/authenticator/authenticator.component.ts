import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { AuthorizationService } from '../../services/authorization.service';
import { HouseService } from '../../services/house.service';

@Component({
  selector: 'app-authenticator',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './authenticator.component.html',
  styleUrl: './authenticator.component.css',
})
export class AuthenticatorComponent implements OnInit {
  constructor(public authorizationService: AuthorizationService,
    private houseService: HouseService
  ) {}

  ngOnInit(): void {
    this.authorizationService.getUserClaims();
  }

  sayHello() {
    console.log('About to say hello');
    this.houseService.sayHello().subscribe(
      {
        next: (h) => console.log(h),
        error: (e) => console.log(e),
        complete: () => console.log('Complete'),
      }
    );
  };
}
