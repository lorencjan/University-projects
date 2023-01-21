package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.*;

import javax.persistence.*;
import java.util.*;

@Entity
@Table(name = "book")
public class Book extends Title{

    @Column(name = "ISBN", nullable = false)
    private String isbn;

    @Column(name = "publicationNumber", nullable = false)
    private int publicationNumber;

    @Column(name = "publicationDate", nullable = false)
    private Date publicationDate;

    @Column(name = "numberOfPages", nullable = false)
    private int numberOfPages;

    @OneToMany(fetch = FetchType.LAZY, mappedBy = "book", orphanRemoval = true)
    private List<HardCopyExemplar> hardCopyExemplars = new ArrayList<>();

    @OneToMany(fetch = FetchType.LAZY, mappedBy = "book", orphanRemoval = true)
    private List<ElectronicCopyExemplar> electronicCopyExemplars = new ArrayList<>();

    @ManyToMany(fetch = FetchType.LAZY, cascade = { CascadeType.PERSIST, CascadeType.MERGE })
    @JoinTable(name = "bookgenre", joinColumns = { @JoinColumn(name = "bookId", referencedColumnName = "ID") }, inverseJoinColumns = { @JoinColumn(name = "genreId", referencedColumnName = "ID") })
    private List<Genre> genres = new ArrayList<>();

    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(name = "bookauthor", joinColumns = { @JoinColumn(name = "bookId", referencedColumnName = "ID") }, inverseJoinColumns = { @JoinColumn(name = "authorId", referencedColumnName = "ID") })
    private List<Author> authors = new ArrayList<>();

    public Book(){}

    public Book(Long id){ this.id = id; }

//    public Book(Long id, String name, String description, String publisher, String language, String coverPhotoPath, String isbn, int publicationNumber, Date publicationDate, int numberOfPages,
//                List<Author> authors, List<Genre> genres, List<HardCopyExemplar> hardCopyExemplars, List<ElectronicCopyExemplar> electronicCopyExemplars) {
    public Book(Long id, String name, String description, String publisher, String language, String coverPhotoPath, String isbn, int publicationNumber, Date publicationDate, int numberOfPages,
             List<Author> authors, List<Genre> genres, List<HardCopyExemplar> hardCopyExemplars, List<ElectronicCopyExemplar> electronicCopyExemplars) {
        super(id, name, description, publisher, language, coverPhotoPath);
        this.isbn = isbn;
        this.publicationNumber = publicationNumber;
        this.publicationDate = publicationDate;
        this.numberOfPages = numberOfPages;
        this.authors = authors;
        this.genres = genres;
        this.hardCopyExemplars = hardCopyExemplars;
        this.electronicCopyExemplars = electronicCopyExemplars;
    }

    public Book( String name, String description, String publisher, String language, String coverPhotoPath, String isbn, int publicationNumber, Date publicationDate, int numberOfPages,
                List<Author> authors, List<Genre> genres, List<HardCopyExemplar> hardCopyExemplars, List<ElectronicCopyExemplar> electronicCopyExemplars) {
        super(name, description, publisher, language, coverPhotoPath);
        this.isbn = isbn;
        this.publicationNumber = publicationNumber;
        this.publicationDate = publicationDate;
        this.numberOfPages = numberOfPages;
        this.authors = authors;
        this.genres = genres;
        this.hardCopyExemplars = hardCopyExemplars;
        this.electronicCopyExemplars = electronicCopyExemplars;
    }


    public String getIsbn() {
        return isbn;
    }

    public void setIsbn(String isbn) {
        this.isbn = isbn;
    }

    public int getPublicationNumber() {
        return publicationNumber;
    }

    public void setPublicationNumber(int publicationNumber) {
        this.publicationNumber = publicationNumber;
    }

    public Date getPublicationDate() {
        return publicationDate;
    }

    public void setPublicationDate(Date publicationDate) {
        this.publicationDate = publicationDate;
    }

    public int getNumberOfPages() {
        return numberOfPages;
    }

    public void setNumberOfPages(int numberOfPages) {
        this.numberOfPages = numberOfPages;
    }

    public List<Genre> getGenres() {
        return genres;
    }

    public void setGenres(List<Genre> genres) {
        this.genres = genres;
    }

    public List<Author> getAuthors() {
        return authors;
    }

    public void setAuthors(List<Author> authors) {
        this.authors = authors;
    }

    public void addGenres(Genre genre) {
        genres.add(genre);
        genre.getBooks().add(this);
    }

    public void removeGenres(Genre genre) {
        genres.remove(genre);
        genre.getBooks().remove(this);
    }

    public void addAuthors(Author author) {
        authors.add(author);
        author.getBooks().add(this);
    }

    public void removeAuthors(Author author) {
        authors.remove(author);
        author.getBooks().remove(this);
    }

    public List<HardCopyExemplar> getHardCopyExemplars() {
        return hardCopyExemplars;
    }

    public void setHardCopyExemplars(List<HardCopyExemplar> hardCopyExemplars) {
        this.hardCopyExemplars = hardCopyExemplars;
    }

    public List<ElectronicCopyExemplar> getElectronicCopyExemplars() {
        return electronicCopyExemplars;
    }

    public void setElectronicCopyExemplars(List<ElectronicCopyExemplar> electronicCopyExemplars) {
        this.electronicCopyExemplars = electronicCopyExemplars;
    }

    public BookDto toDto() {
        List<AuthorDto> authors = getAuthors().stream().map(x -> x.toDto()).toList();
        List<ExemplarLightDto> hardCopyExemplars = getHardCopyExemplars().stream().map(x -> x.toLightDto()).toList();
        List<ExemplarLightDto> electronicCopyExemplars = getElectronicCopyExemplars().stream().map(x -> x.toLightDto()).toList();
        List<GenreDto> genres = getGenres().stream().map(x -> x.toDto()).toList();
        return new BookDto(id, name, description, publisher, language, coverPhotoPath, authors, hardCopyExemplars, electronicCopyExemplars,
                isbn, publicationNumber, publicationDate, numberOfPages, genres );
    }
}
