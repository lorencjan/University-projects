package com.fit.vut.Library.dtos;

public class ChangePasswordDto {

    protected String userEmail;

    protected String password;

    public ChangePasswordDto() {
    }

    public ChangePasswordDto(String userEmail, String password) {
        this.userEmail = userEmail;
        this.password = password;
    }

    public String getUserEmail() {
        return userEmail;
    }

    public void setUserEmail(String userEmail) {
        this.userEmail = userEmail;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }
}
