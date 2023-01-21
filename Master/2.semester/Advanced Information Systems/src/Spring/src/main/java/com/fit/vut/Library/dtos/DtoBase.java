package com.fit.vut.Library.dtos;

public abstract class DtoBase {

    protected Long id;

    public DtoBase () {}

    public DtoBase (Long id) { this.id = id; }


    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }
}
