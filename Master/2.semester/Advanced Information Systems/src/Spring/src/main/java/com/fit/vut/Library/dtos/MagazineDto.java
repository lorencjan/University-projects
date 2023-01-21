package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;
import java.util.*;

public class MagazineDto extends TitleDto {

    private Long issn;

    private int number;

    private int publicationYear;

    private List<FieldDto> fields;

    public MagazineDto() {}

    public MagazineDto(Long id, String name, String description, String publisher, String language, String coverPhotoPath, List<AuthorDto> authors, List<ExemplarLightDto> hardCopyExemplars,
                       List<ExemplarLightDto> electronicCopyExemplars, Long issn, int number, int publicationYear, List<FieldDto> fields) {
        super(id, name, description, publisher, language, coverPhotoPath, authors, hardCopyExemplars, electronicCopyExemplars);
        this.issn = issn;
        this.number = number;
        this.publicationYear = publicationYear;
        this.fields = fields;
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

    public int getPublicationYear() {
        return publicationYear;
    }

    public void setPublicationYear(int publicationYear) {
        this.publicationYear = publicationYear;
    }

    public List<FieldDto> getFields() {
        return fields;
    }

    public void setFields(List<FieldDto> fields) {
        this.fields = fields;
    }

    public Magazine toEntity() {
        List<Author> authorEntities = getAuthors() == null ? new ArrayList() : getAuthors().stream().map(x -> x.toEntity()).toList();
        List<Field> fieldEntities = getFields() == null ? new ArrayList() : getFields().stream().map(x -> x.toEntity()).toList();
        List<HardCopyExemplar> hardCopyExemplars = getHardCopyExemplars() == null ? new ArrayList() : getHardCopyExemplars().stream().map(x -> x.toHardCopyExemplarEntity()).toList();
        List<ElectronicCopyExemplar> electronicCopyExemplars = getElectronicCopyExemplars()== null ? new ArrayList() : getElectronicCopyExemplars().stream().map(x -> x.toElectronicCopyExemplarEntity()).toList();
        return new Magazine(id, name, description, publisher, language, coverPhotoPath, issn, number, publicationYear,
                authorEntities, fieldEntities, hardCopyExemplars, electronicCopyExemplars);
    }
}
