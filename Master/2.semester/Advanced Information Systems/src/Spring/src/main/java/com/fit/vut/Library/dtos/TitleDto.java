package com.fit.vut.Library.dtos;

import java.util.List;

public abstract class TitleDto extends DtoBase {

    protected String name;

    protected String description;

    protected String publisher;

    protected String language;

    protected String coverPhotoPath;

    protected List<AuthorDto> authors;

    protected List<ExemplarLightDto> hardCopyExemplars;

    protected List<ExemplarLightDto> electronicCopyExemplars;

    public TitleDto() {}

    public TitleDto(Long id, String name, String description, String publisher, String language, String coverPhotoPath,
                    List<AuthorDto> authors, List<ExemplarLightDto> hardCopyExemplars, List<ExemplarLightDto> electronicCopyExemplars) {
        super(id);
        this.name = name;
        this.description = description;
        this.publisher = publisher;
        this.language = language;
        this.coverPhotoPath = coverPhotoPath;
        this.authors = authors;
        this.hardCopyExemplars = hardCopyExemplars;
        this.electronicCopyExemplars = electronicCopyExemplars;
    }


    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getPublisher() {
        return publisher;
    }

    public void setPublisher(String publisher) {
        this.publisher = publisher;
    }

    public String getCoverPhotoPath() {
        return coverPhotoPath;
    }

    public void setCoverPhotoPath(String coverPhotoPath) {
        this.coverPhotoPath = coverPhotoPath;
    }

    public String getLanguage() { return language; }

    public void setLanguage(String language) {
        this.language = language;
    }

    public List<AuthorDto> getAuthors() { return authors; }

    public void setAuthors(List<AuthorDto> authors) {
        this.authors = authors;
    }

    public List<ExemplarLightDto> getHardCopyExemplars() {
        return hardCopyExemplars;
    }

    public void setHardCopyExemplars(List<ExemplarLightDto> exemplars) { this.hardCopyExemplars = exemplars; }

    public List<ExemplarLightDto> getElectronicCopyExemplars() {
        return electronicCopyExemplars;
    }

    public void setElectronicCopyExemplars(List<ExemplarLightDto> exemplars) { this.electronicCopyExemplars = exemplars; }
}
