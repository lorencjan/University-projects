package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;

public class TitleLightDto extends DtoBase {

    private String name;

    private String description;

    private String language;

    private String coverPhotoPath;

    public TitleLightDto() {}

    public TitleLightDto(Long id, String name, String description, String language, String coverPhotoPath) {
        super(id);
        this.name = name;
        this.description = description;
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

    public String getCoverPhotoPath() {
        return coverPhotoPath;
    }

    public void setCoverPhotoPath(String coverPhotoPath) {
        this.coverPhotoPath = coverPhotoPath;
    }

    public String getLanguage() {
        return language;
    }

    public void setLanguage(String language) {
        this.language = language;
    }

    public Book toBookEntity() { return new Book(id);}

    public Magazine toMagazineEntity() { return new Magazine(id);}
}
