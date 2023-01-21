package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.Reader;

public class ReaderLightDto extends DtoBase {

    public String fullname;

    public ReaderLightDto() {}

    public ReaderLightDto(Long id, String name, String surname) {
        super(id);
        this.fullname = name + " " + surname;
    }

    public String getFullname() { return fullname; }

    public void setFullname(String fullname) { this.fullname = fullname; }

    public Reader toEntity() { return new Reader(id); }
}
