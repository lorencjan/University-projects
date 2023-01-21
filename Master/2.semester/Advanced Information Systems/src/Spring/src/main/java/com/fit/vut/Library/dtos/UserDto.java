package com.fit.vut.Library.dtos;

import com.fit.vut.Library.enums.RoleEnum;

public abstract class UserDto extends DtoBase {

    protected String name;

    protected String surname;

    protected String email;

    protected String city;

    protected String street;

    protected String houseNumber;

    protected String postcode;

    protected RoleEnum role;

    public UserDto() {}

    public UserDto(Long id, String name, String surname, String email, String city, String street, String houseNumber,
                   String postcode, RoleEnum role) {
        super(id);
        this.name = name;
        this.surname = surname;
        this.email = email;
        this.city = city;
        this.street = street;
        this.houseNumber = houseNumber;
        this.postcode = postcode;
        this.role = role;
    }

    public String getName() { return name; }

    public void setName(String name) { this.name = name; }

    public String getSurname() { return surname; }

    public void setSurname(String surname) { this.surname = surname; }

    public String getFullname() { return name + " " + surname; }

    public String getEmail() { return email; }

    public void setEmail(String email) { this.email = email; }

    public String getCity() { return city; }

    public void setCity(String city) { this.city = city; }

    public String getStreet() { return street; }

    public void setStreet(String street) { this.street = street; }

    public String getHouseNumber() { return houseNumber; }

    public void setHouseNumber(String houseNumber) { this.houseNumber = houseNumber; }

    public String getPostcode() { return postcode; }

    public void setPostcode(String postcode) { this.postcode = postcode; }

    public RoleEnum getRole() { return role; }

    public void setRole(RoleEnum role) { this.role = role; }
}
