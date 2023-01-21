package com.fit.vut.Library.entities;

import javax.persistence.*;

@MappedSuperclass
public abstract class EntityBase {

    @Id
    @Column(name = "id", nullable = false)
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    protected Long id;

    public EntityBase() {}

    public EntityBase(Long id) {
        this.id = id;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }
}
