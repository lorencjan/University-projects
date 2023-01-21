package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;
import java.util.*;

public class BookDto extends TitleDto {

    private String isbn;

    private int publicationNumber;

    private Date publicationDate;

    private int pages;

    private List<GenreDto> genres;

    public BookDto() {}

    public BookDto(Long id, String name, String description, String publisher, String language, String coverPhotoPath, List<AuthorDto> authors, List<ExemplarLightDto> hardCopyExemplars,
                   List<ExemplarLightDto> electronicCopyExemplars, String isbn, int publicationNumber, Date publicationDate, int pages, List<GenreDto> genres) {
        super(id, name, description, publisher, language, coverPhotoPath, authors, hardCopyExemplars, electronicCopyExemplars);
        this.isbn = isbn;
        this.publicationNumber = publicationNumber;
        this.publicationDate = publicationDate;
        this.pages = pages;
        this.genres = genres;
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

    public int getPages() {
        return pages;
    }

    public void setPages(int pages) {
        this.pages = pages;
    }

    public List<GenreDto> getGenres() {
        return genres;
    }

    public void setGenres(List<GenreDto> genres) {
        this.genres = genres;
    }

    public Book toEntity() {
        List<Author> authorEntities = getAuthors() == null ? new ArrayList() : getAuthors().stream().map(x -> x.toEntity()).toList();
        List<Genre> genreEntities = getGenres() == null ? new ArrayList() : getGenres().stream().map(x -> x.toEntity()).toList();
        List<HardCopyExemplar> hardCopyExemplars = getHardCopyExemplars() == null ? new ArrayList() : getHardCopyExemplars().stream().map(x -> x.toHardCopyExemplarEntity()).toList();
        List<ElectronicCopyExemplar> electronicCopyExemplars = getElectronicCopyExemplars()== null ? new ArrayList() : getElectronicCopyExemplars().stream().map(x -> x.toElectronicCopyExemplarEntity()).toList();
        return new Book(id, name, description, publisher, language, coverPhotoPath, isbn, publicationNumber, publicationDate, pages,
                authorEntities, genreEntities, hardCopyExemplars, electronicCopyExemplars);
    }
}
