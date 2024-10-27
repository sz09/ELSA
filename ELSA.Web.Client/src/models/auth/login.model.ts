export class LoginModel {
    constructor(username: string, password: string, keepMeLogIn: boolean) {
        this.username = username;
        this.password = password;
        this.keepMeLogIn = keepMeLogIn;
    }
    username!: string;
    password!: string;
    keepMeLogIn: boolean = false;
}