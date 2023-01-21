package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.Genre;

public class GenreDto extends DtoBase {

    private String name;

    public GenreDto(){}

    public GenreDto(Long id, String name){
        super(id);
        this.name = name;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Genre toEntity() { return new Genre(id, name); }
}
