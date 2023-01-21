package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.FieldDto;

import javax.persistence.*;
import java.util.*;

@Entity
@Table(name = "field")
public class Field extends EntityBase{

    @Column(name = "name", nullable = false, unique = true)
    private String name;

    @ManyToMany(mappedBy = "fields")
    private List<Magazine> magazines = new ArrayList<>();

    public Field() {}

    public Field(long id) {
        this.id = id;
    }

    public Field(String name) {
        this.name = name;
    }

    public Field(long id, String name) {
        this.id = id;
        this.name = name;
    }


    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public List<Magazine> getMagazines() {
        return magazines;
    }

    public void setMagazines(List<Magazine> magazines) {
        this.magazines = magazines;
    }

    public void addMagazines(Magazine magazine) {
        magazines.add(magazine);
        magazine.getFields().add(this);
    }

    public void removeMagazines(Magazine magazine) {
        magazines.remove(magazine);
        magazine.getFields().remove(this);
    }

    public FieldDto toDto() { return new FieldDto(id, name); }
}
