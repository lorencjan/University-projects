package com.fit.vut.Library.dtos;

import com.fit.vut.Library.enums.RoleEnum;

public class EmployeeDto extends UserDto {

    public EmployeeDto() {}

    public EmployeeDto(Long id, String name, String surname, String email, String city, String street, String houseNumber, String postcode, RoleEnum role) {
        super(id, name, surname, email, city, street, houseNumber, postcode, role);
    }
}
