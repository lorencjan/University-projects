package com.fit.vut.Library.dtos;

public class CredentialsDto {

    protected String email;

    protected String password;

    public CredentialsDto() {}

    public CredentialsDto(String email, String password) {
        this.email = email;
        this.password = password;
    }


    public String getEmail() { return email; }

    public void setEmail(String email) { this.email = email; }

    public String getPassword() { return password; }

    public void setPassword(String password) { this.password = password; }
}
