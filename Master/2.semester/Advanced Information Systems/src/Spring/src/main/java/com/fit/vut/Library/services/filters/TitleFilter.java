package com.fit.vut.Library.services.filters;

public class TitleFilter {

    protected String name;

    protected String publisher;

    protected String language;

    protected String author;

    public TitleFilter(String name, String publisher, String language, String author){
        this.name = name;
        this.publisher = publisher;
        this.language = language;
        this.author = author;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getPublisher() {
        return publisher;
    }

    public void setPublisher(String publisher) {
        this.publisher = publisher;
    }

    public String getLanguage() { return language; }

    public void setLanguage(String language) {
        this.language = language;
    }

    public String getAuthor() { return author; }

    public void setAuthor(String author) {
        this.author = author;
    }
}
