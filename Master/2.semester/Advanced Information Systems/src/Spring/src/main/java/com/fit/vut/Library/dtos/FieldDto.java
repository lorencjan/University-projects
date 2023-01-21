package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.Field;

public class FieldDto extends DtoBase {

    private String name;

    public FieldDto(){}

    public FieldDto(Long id, String name){
        super(id);
        this.name = name;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Field toEntity() { return new Field(id, name); }
}
