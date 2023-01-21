package com.fit.vut.Library.services.filters;

import java.util.List;
import java.util.Optional;

public class MagazineFilter extends TitleFilter {

    private String issn;

    private Optional<Integer> year;

    private Optional<Integer> number;

    private List<String> fields;

    public MagazineFilter(String name, String publisher, String language, String author, String issn, Optional<Integer> year, Optional<Integer> number, List<String> fields){
        super(name, publisher, language, author);
        this.issn = issn;
        this.fields = fields;
        this.year = year;
        this.number = number;
    }

    public String getIssn() { return issn; }

    public void setIssn(String issn) { this.issn = issn; }

    public Optional<Integer> getYear() { return year; }

    public void setYear(Optional<Integer> year) { this.year = year; }

    public Optional<Integer> getNumber() { return number; }

    public void setNumber(Optional<Integer> number) { this.number = number; }

    public List<String> getFields() {
        return fields;
    }

    public void setFields(List<String> genres) {
        this.fields = genres;
    }
}
