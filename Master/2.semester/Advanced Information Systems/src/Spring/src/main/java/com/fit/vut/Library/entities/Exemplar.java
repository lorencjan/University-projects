package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.ExemplarLightDto;
import com.fit.vut.Library.dtos.TitleLightDto;
import com.fit.vut.Library.enums.ExemplarEnum;

import javax.persistence.*;

@MappedSuperclass
public abstract class Exemplar extends EntityBase {

    @Column(name = "state", nullable = false)
    @Enumerated(EnumType.STRING)
    protected ExemplarEnum state;

    @Column(name = "availability", nullable = false)
    protected boolean availability;

    @Column(name = "borrowPeriod", nullable = false)
    protected int borrowPeriod;

    @Column(name = "maximumNumberOfExtension", nullable = false)
    protected int maximumNumberOfExtension;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="bookId")
    protected Book book;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="magazineId")
    protected Magazine magazine;

    public Exemplar(){}

    public Exemplar(Long id, ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfExtension, Book book, Magazine magazine) {
        super(id);
        this.state = state;
        this.availability = availability;
        this.borrowPeriod = borrowPeriod;
        this.maximumNumberOfExtension = maximumNumberOfExtension;
        this.book = book;
        this.magazine = magazine;
    }

    public Exemplar( ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfExtension, Book book, Magazine magazine) {
        this.state = state;
        this.availability = availability;
        this.borrowPeriod = borrowPeriod;
        this.maximumNumberOfExtension = maximumNumberOfExtension;
        this.book = book;
        this.magazine = magazine;
    }

    public ExemplarEnum getState() {
        return state;
    }

    public void setState(ExemplarEnum state) {
        this.state = state;
    }

    public boolean getAvailability() {
        return availability;
    }

    public void setAvailability(boolean availability) {
        this.availability = availability;
    }

    public int getBorrowPeriod() {
        return borrowPeriod;
    }

    public void setBorrowPeriod(int borrowPeriod) {
        this.borrowPeriod = borrowPeriod;
    }

    public int getMaximumNumberOfExtension() {
        return maximumNumberOfExtension;
    }

    public void setMaximumNumberOfExtension(int maximumNumberOfExtension) {
        this.maximumNumberOfExtension = maximumNumberOfExtension;
    }

    public Book getBook() {
        return book;
    }

    public void setBook(Book book) {
        this.book = book;
    }

    public Magazine getMagazine() {
        return magazine;
    }

    public void setMagazine(Magazine magazine) {
        this.magazine = magazine;
    }

    public ExemplarLightDto toLightDto() {
        TitleLightDto bookDto = book == null ? null : book.toLightDto();
        TitleLightDto magazineDto = magazine == null ? null : magazine.toLightDto();
        return new ExemplarLightDto(id, state, availability, borrowPeriod, maximumNumberOfExtension, bookDto, magazineDto);
    }
}
