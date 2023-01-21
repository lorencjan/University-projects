package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.*;

import javax.persistence.*;
import java.util.*;

@Entity
@Table(name = "author")
public class Author extends EntityBase {

    @Column(name = "name", nullable = false)
    private String name;

    @Column(name = "surname", nullable = false)
    private String surname;

    @Column(name = "photographPath")
    private String photographPath;

    @Lob
    @Column(name = "description")
    protected String description;

    @Column(name = "dateOfBirth")
    private Date dateOfBirth;

    @Column(name = "dateOfDeath")
    private Date dateOfDeath;


    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(name = "bookauthor", joinColumns = { @JoinColumn(name = "authorId", referencedColumnName = "ID") }, inverseJoinColumns = { @JoinColumn(name = "bookId", referencedColumnName = "ID") })
    private List<Book> books = new ArrayList<>();

    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(name = "magazineauthor", joinColumns = { @JoinColumn(name = "authorId", referencedColumnName = "ID") }, inverseJoinColumns = { @JoinColumn(name = "magazineId", referencedColumnName = "ID ") })
    private List<Magazine> magazines = new ArrayList<>();

    public Author(){
    }

    public Author(Long id){
        super(id);
    }

    public Author(Long id, String name, String surname, String photographPath, Date dateOfBirth, List<Book> books,
                  List<Magazine> magazines, String description, Date dateOfDeath) {
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

    public String getPhotographPath() {
        return photographPath;
    }

    public void setPhotographPath(String photographPath) {
        this.photographPath = photographPath;
    }

    public Date getDateOfBirth() {
        return dateOfBirth;
    }

    public void setDateOfBirth(Date dateOfBirth) {
        this.dateOfBirth = dateOfBirth;
    }

    public List<Book> getBooks() {
        return books;
    }

    public void setBooks(List<Book> books) {
        this.books = books;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Date getDateOfDeath() {
        return dateOfDeath;
    }

    public void setDateOfDeath(Date dateOfDeath) {
        this.dateOfDeath = dateOfDeath;
    }

    public List<Magazine> getMagazines() {
        return magazines;
    }

    public void setMagazines(List<Magazine> magazines) {
        this.magazines = magazines;
    }

    public void addMagazines(Magazine magazine) {
        magazines.add(magazine);
        magazine.getAuthors().add(this);
    }

    public void removeMagazines(Magazine magazine) {
        magazines.remove(magazine);
        magazine.getAuthors().remove(this);
    }

    public void addBooks(Book book) {
        books.add(book);
        book.getAuthors().add(this);
    }

    public void removeBooks(Book book) {
        books.remove(book);
        book.getAuthors().remove(this);
    }

    public AuthorDto toDto() {
        List<TitleLightDto> books = getBooks().stream().map(x -> x.toLightDto()).toList();
        List<TitleLightDto> magazines = getMagazines().stream().map(x -> x.toLightDto()).toList();
        return new AuthorDto(id, name, surname, photographPath, dateOfBirth, books, magazines, description, dateOfDeath);
    }
}
