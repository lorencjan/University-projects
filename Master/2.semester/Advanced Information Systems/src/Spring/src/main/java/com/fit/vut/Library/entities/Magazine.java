package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.*;

import java.util.*;
import javax.persistence.*;

@Entity
@Table(name = "magazine")
public class Magazine extends Title{

    @Column(name = "ISSN", nullable = false)
    private Long issn;

    @Column(name = "number", nullable = false)
    private int number;

    @Column(name = "year", nullable = false)
    private int year;

    @OneToMany(fetch = FetchType.LAZY, mappedBy = "magazine", orphanRemoval = true)
    private List<HardCopyExemplar> hardCopyExemplars = new ArrayList<>();

    @OneToMany(fetch = FetchType.LAZY, mappedBy = "magazine", orphanRemoval = true)
    private List<ElectronicCopyExemplar> electronicCopyExemplars = new ArrayList<>();

    @ManyToMany(fetch = FetchType.LAZY, cascade = { CascadeType.PERSIST, CascadeType.MERGE })
    @JoinTable(name = "magazinefield", joinColumns = { @JoinColumn(name = "magazineID", referencedColumnName = "ID") }, inverseJoinColumns = { @JoinColumn(name = "fieldId", referencedColumnName = "ID") })
    private List<Field> fields = new ArrayList<>();

    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(name = "magazineauthor", joinColumns = { @JoinColumn(name = "magazineId", referencedColumnName = "ID") }, inverseJoinColumns = { @JoinColumn(name = "authorId", referencedColumnName = "ID") })
    private List<Author> authors = new ArrayList<>();

    public Magazine() { }

    public Magazine(Long id){ this.id = id; }

    public Magazine(Long id, String name, String description, String publisher, String language, String coverPhotoPath,
                    Long issn, int number, int year, List<Author> authors, List<Field> fields,
                    List<HardCopyExemplar> hardCopyExemplars, List<ElectronicCopyExemplar> electronicCopyExemplars) {
        super(id, name, description, publisher, language, coverPhotoPath);
        this.issn = issn;
        this.number = number;
        this.year = year;
        this.authors = authors;
        this.fields = fields;
        this.hardCopyExemplars = hardCopyExemplars;
        this.electronicCopyExemplars = electronicCopyExemplars;
    }

    public Magazine(String name, String description, String publisher, String language, String coverPhotoPath,
                    Long issn, int number, int year, List<Author> authors, List<Field> fields,
                    List<HardCopyExemplar> hardCopyExemplars, List<ElectronicCopyExemplar> electronicCopyExemplars) {
        super(name, description, publisher, language, coverPhotoPath);
        this.issn = issn;
        this.number = number;
        this.year = year;
        this.authors = authors;
        this.fields = fields;
        this.hardCopyExemplars = hardCopyExemplars;
        this.electronicCopyExemplars = electronicCopyExemplars;
    }


    public Long getIssn() {
        return issn;
    }

    public void setIssn(Long issn) {
        this.issn = issn;
    }

    public int getNumber() {
        return number;
    }

    public void setNumber(int number) {
        this.number = number;
    }

    public int getYear() {
        return year;
    }

    public void setYear(int year) {
        this.year = year;
    }

    public List<Field> getFields() {
        return fields;
    }

    public void setFields(List<Field> fields) {
        this.fields = fields;
    }

    public List<Author> getAuthors() {
        return authors;
    }

    public void setAuthors(List<Author> authors) {
        this.authors = authors;
    }

    public void addFields(Field field) {
        fields.add(field);
        field.getMagazines().add(this);
    }

    public void removeFields(Field field) {
        fields.remove(field);
        field.getMagazines().remove(this);
    }

    public void addAuthors(Author author) {
        authors.add(author);
        author.getMagazines().add(this);
    }

    public void removeAuthors(Author author) {
        authors.remove(author);
        author.getMagazines().remove(this);
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

    public MagazineDto toDto() {
        List<AuthorDto> authors = getAuthors().stream().map(x -> x.toDto()).toList();
        List<ExemplarLightDto> hardCopyExemplars = getHardCopyExemplars().stream().map(x -> x.toLightDto()).toList();
        List<ExemplarLightDto> electronicCopyExemplars = getElectronicCopyExemplars().stream().map(x -> x.toLightDto()).toList();
        List<FieldDto> fields = getFields().stream().map(x -> x.toDto()).toList();
        return new MagazineDto(id, name, description, publisher, language, coverPhotoPath, authors, hardCopyExemplars, electronicCopyExemplars,
                issn, number, year, fields);
    }
}
