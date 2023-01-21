package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.TitleLightDto;

import javax.persistence.*;
import java.util.*;

@MappedSuperclass
public abstract class Title extends EntityBase {

    @Column(name = "name", nullable = false)
    protected String name;

    @Lob
    @Column(name = "description")
    protected String description;

    @Column(name = "publisher", nullable = false)
    protected String publisher;

    @Column(name = "language", nullable = false)
    protected String language;

    @Column(name = "coverPhotoPath")
    protected String coverPhotoPath;

    public Title() {
    }

    public Title(Long id, String name, String description, String publisher, String language, String coverPhotoPath) {
        super(id);
        this.name = name;
        this.description = description;
        this.publisher = publisher;
        this.language = language;
        this.coverPhotoPath = coverPhotoPath;
    }

    public Title(String name, String description, String publisher, String language, String coverPhotoPath) {
        this.name = name;
        this.description = description;
        this.publisher = publisher;
        this.language = language;
        this.coverPhotoPath = coverPhotoPath;
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

    public String getLanguage() {
        return language;
    }

    public void setLanguage(String language) {
        this.language = language;
    }

    public String getCoverPhotoPath() {
        return coverPhotoPath;
    }

    public void setCoverPhotoPath(String coverPhotoPath) {
        this.coverPhotoPath = coverPhotoPath;
    }

    public TitleLightDto toLightDto() { return new TitleLightDto(id, name, description, language, coverPhotoPath);}
}
