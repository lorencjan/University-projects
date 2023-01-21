package com.fit.vut.Library.services.filters;

import java.util.List;

public class BookFilter extends TitleFilter {

    private String isbn;

    private List<String> genres;

    public BookFilter(String name, String publisher, String language, String author, String isbn, List<String> genres){
        super(name, publisher, language, author);
        this.isbn = isbn;
        this.genres = genres;
    }

    public String getIsbn() {
        return isbn;
    }

    public void setIsbn(String isbn) {
        this.isbn = isbn;
    }

    public List<String> getGenres() {
        return genres;
    }

    public void setGenres(List<String> genres) {
        this.genres = genres;
    }
}
