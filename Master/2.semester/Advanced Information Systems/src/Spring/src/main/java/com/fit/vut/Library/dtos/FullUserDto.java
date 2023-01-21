package com.fit.vut.Library.dtos;

import com.fit.vut.Library.enums.RoleEnum;

public class FullUserDto extends UserDto {

    protected String password;

    public FullUserDto() {}

    public FullUserDto(Long id, String name, String surname, String email, String city, String street, String houseNumber, String postcode, RoleEnum role) {
        super(id, name, surname, email, city, street, houseNumber, postcode, role);
        this.password = password;
    }

    public String getPassword() { return password; }

    public void setPassword(String password) { this.password = password; }
}
