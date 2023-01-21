package com.fit.vut.Library.entities;

import com.fit.vut.Library.enums.RoleEnum;

import javax.persistence.*;

@MappedSuperclass
public abstract class User extends EntityBase {

    @Column(name = "email", nullable = false)
    protected String email;

    @Column(name = "hashPassword", nullable = false)
    protected String hashPassword;

    @Column(name = "name", nullable = false)
    protected String name;

    @Column(name = "surname", nullable = false)
    protected String surname;

    @Column(name = "street")
    protected String street;

    @Column(name = "houseNumber")
    protected String houseNumber;

    @Column(name = "city")
    protected String city;

    @Column(name = "postcode")
    protected String postcode;

    @Column(name = "role", nullable = false)
    @Enumerated(EnumType.STRING)
    protected RoleEnum role;

    public User(){
    }

    public User(Long id, String email, String hashPassword, String name, String surname, String street,
                String houseNumber, String city, String postcode, RoleEnum role) {
        super(id);
        this.email = email;
        this.hashPassword = hashPassword;
        this.name = name;
        this.surname = surname;
        this.street = street;
        this.houseNumber = houseNumber;
        this.city = city;
        this.postcode = postcode;
        this.role = role;
    }

    public User(String email, String hashPassword, String name, String surname,
                String street, String houseNumber, String city, String postcode,
                RoleEnum role) {
        this.email = email;
        this.hashPassword = hashPassword;
        this.name = name;
        this.surname = surname;
        this.street = street;
        this.houseNumber = houseNumber;
        this.city = city;
        this.postcode = postcode;
        this.role = role;
    }


    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getHashPassword() {
        return hashPassword;
    }

    public void setHashPassword(String hashPassword) {
        this.hashPassword = hashPassword;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getSurname() {
        return surname;
    }

    public void setSurname(String surname) {
        this.surname = surname;
    }

    public String getStreet() {
        return street;
    }

    public void setStreet(String street) {
        this.street = street;
    }

    public String getHouseNumber() {
        return houseNumber;
    }

    public void setHouseNumber(String houseNumber) {
        this.houseNumber = houseNumber;
    }

    public String getCity() {
        return city;
    }

    public void setCity(String city) {
        this.city = city;
    }

    public String getPostcode() {
        return postcode;
    }

    public void setPostcode(String postcode) {
        this.postcode = postcode;
    }

    public RoleEnum getRole() {
        return role;
    }

    public void setRole(RoleEnum role) {
        this.role = role;
    }
}
