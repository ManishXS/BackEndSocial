import { Component, OnInit } from '@angular/core';
import { UserService } from './services/user.service';
import { UserData } from './models/user-data.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  generatedUserData: UserData = new UserData('', '', ''); // Initialize UserData object with profilePic

  constructor(private userService: UserService) {}

  ngOnInit() {
    const userId = this.getCookie('userId');
    const username = this.getCookie('username');
    const profilePic = this.getCookie('profilePic'); // Retrieve profile pic from cookies

    // If all cookies are found, use them.
    if (userId && username && profilePic) {
      //console.log('Existing cookies found. Using existing user data.');
      this.generatedUserData = new UserData(userId, username, profilePic);
      //console.log('User data from cookies:', this.generatedUserData);
    } else {
      // If cookies are not found, generate new user data via the API
      //console.log('No valid cookies found, generating new user data via API.');
      this.generateAndStoreUser(true); // Generate new data if no cookies
    }
  }

  generateAndStoreUser(storeCookies: boolean) {
    //console.log('Calling API to generate user data...');
    this.userService.generateUserId().subscribe(
      (response: UserData) => {
        this.generatedUserData = response;
        //console.log('Generated User ID:', this.generatedUserData.userId);
        //console.log('Generated Username:', this.generatedUserData.username);
        //console.log('Generated Profile Pic URL:', this.generatedUserData.profilePic);

        // Set new cookies with the generated data, including profilePic
        if (storeCookies) {
          this.setCookie('userId', this.generatedUserData.userId, 365);
          this.setCookie('username', this.generatedUserData.username, 365);
          this.setCookie('profilePic', this.generatedUserData.profilePic, 365); // Store profile pic URL in cookies
          //console.log('New cookies set for userId, username, and profilePic.');
        }
      },
      error => {
        console.error('Error generating user ID:', error);
      }
    );
  }

  // Function to set a cookie with logging
  setCookie(name: string, value: string, days: number) {
    const d = new Date();
    d.setTime(d.getTime() + (days * 24 * 60 * 60 * 1000));
    const expires = "expires=" + d.toUTCString();
    document.cookie = `${name}=${value};${expires};path=/`;
    //console.log(`Cookie set: ${name} = ${value}`);
  }

  // Function to get a cookie by name with logging
  getCookie(name: string): string | null {
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i].trim();
      if (c.indexOf(nameEQ) === 0) {
        const value = c.substring(nameEQ.length, c.length);
        //console.log(`Cookie retrieved: ${name} = ${value}`);
        return value;
      }
    }
    return null;
  }
}
