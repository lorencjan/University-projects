package com.fit.vut.Library.dtos;

public class NewPasswordDto {

    protected Long userId;

    protected String password;

    protected String oldPassword;

    public NewPasswordDto() {}

    public NewPasswordDto(Long userId, String password) {
        this.userId = userId;
        this.password = password;
    }

    public NewPasswordDto(Long userId, String password, String oldPassword) {
        this.userId = userId;
        this.password = password;
        this.oldPassword = oldPassword;
    }

    public Long getUserId() { return userId; }

    public void setUserId(Long userId) { this.userId = userId; }

    public String getPassword() { return password; }

    public void setPassword(String password) { this.password = password; }

    public String getOldPassword() {
        return oldPassword;
    }

    public void setOldPassword(String oldPassword) {
        this.oldPassword = oldPassword;
    }
}
