package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.EmployeeDto;
import com.fit.vut.Library.enums.RoleEnum;

import javax.persistence.*;

@Entity
@Table(name = "employee")
public class Employee extends User {

    public Employee() {}

    public Employee(Long id) {
        this.id = id;
    }

    public Employee(Long id, String email, String hashPassword, String name,
                    String surname, String street, String houseNumber, String city,
                    String postcode, RoleEnum role) {
        super(id, email, hashPassword, name, surname, street, houseNumber, city, postcode, role);
    }

    public Employee(String email, String hashPassword, String name, String surname,
                    String street, String houseNumber, String city, String postcode,
                    RoleEnum role) {
        super(email, hashPassword, name, surname, street, houseNumber, city, postcode, role);
    }
    public EmployeeDto toDto() {
        return new EmployeeDto(id, name, surname, email, city, street, houseNumber, postcode, role);
    }
}
