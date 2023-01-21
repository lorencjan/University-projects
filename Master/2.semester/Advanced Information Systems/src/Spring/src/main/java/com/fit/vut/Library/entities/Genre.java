package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.GenreDto;

import javax.persistence.*;
import java.util.*;

@Entity
@Table(name = "genre")
public class Genre extends EntityBase{

    @Column(name = "name", nullable = false, unique = true)
    private String name;

    @ManyToMany(mappedBy = "genres")
    private List<Book> books = new ArrayList<>();

    public Genre() {}

    public Genre(long id) {
        this.id = id;
    }

    public Genre(String name) {
        this.name = name;
    }

    public Genre(long id, String name) {
        this.id = id;
        this.name = name;
    }


    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public List<Book> getBooks() {
        return books;
    }

    public void setBooks(List<Book> books) {
        this.books = books;
    }

    public void addBooks(Book book) {
        books.add(book);
        book.getGenres().add(this);
    }

    public void removeBooks(Book book) {
        books.remove(book);
        book.getGenres().remove(this);
    }

    public GenreDto toDto() { return new GenreDto(id, name); }
}
