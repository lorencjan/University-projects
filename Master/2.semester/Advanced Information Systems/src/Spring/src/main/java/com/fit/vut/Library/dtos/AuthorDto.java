package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;
import java.util.*;

public class AuthorDto extends DtoBase{

    private String name;

    private String surname;

    private String photographPath;

    private Date dateOfBirth;

    private Date dateOfDeath;

    private String description;

    private List<TitleLightDto> books;

    private List<TitleLightDto> magazines;

    public AuthorDto() {}

    public AuthorDto(Long id, String name, String surname, String photographPath, Date dateOfBirth,
                     List<TitleLightDto> books, List<TitleLightDto> magazines, String description, Date dateOfDeath) {
        super(id);
        this.name = name;
        this.surname = surname;
        this.photographPath = photographPath;
        this.dateOfBirth = dateOfBirth;
        this.books = books;
        this.magazines = magazines;
        this.description = description;
        this.dateOfDeath = dateOfDeath;
    }

    public String getName() { return name; }

    public void setName(String name) { this.name = name; }

    public String getSurname() { return surname; }

    public void setSurname(String surname) { this.surname = surname; }

    public String getPhotographPath() {
        return photographPath;
    }

    public void setPhotographPath(String photographPath) {
        this.photographPath = photographPath;
    }

    public Date getDateOfBirth() { return dateOfBirth; }

    public void setDateOfBirth(Date dateOfBirth) { this.dateOfBirth = dateOfBirth; }

    public List<TitleLightDto> getBooks() { return books;  }

    public void setBooks(List<TitleLightDto> books) { this.books = books; }

    public List<TitleLightDto> getMagazines() { return magazines;  }

    public void setMagazines(List<TitleLightDto> magazines) { this.magazines = magazines; }

    public Date getDateOfDeath() {
        return dateOfDeath;
    }

    public void setDateOfDeath(Date dateOfDeath) {
        this.dateOfDeath = dateOfDeath;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Author toEntity() {
        List<Book> books = getBooks() == null ? new ArrayList() : getBooks().stream().map(x -> x.toBookEntity()).toList();
        List<Magazine> magazines = getMagazines() == null ? new ArrayList() : getMagazines().stream().map(x -> x.toMagazineEntity()).toList();
        return new Author(id, name, surname, photographPath, dateOfBirth, books, magazines, description, dateOfDeath);
    }
}
