package com.fit.vut.Library.dtos;


import com.fit.vut.Library.enums.ExemplarEnum;

public abstract class ExemplarBaseDto extends DtoBase {

    protected ExemplarEnum state;

    protected boolean availability;

    protected int borrowPeriod;

    protected int maximumNumberOfExtension;

    protected TitleLightDto book;

    protected TitleLightDto magazine;

    public ExemplarBaseDto() {}

    public ExemplarBaseDto(Long id, ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfExtension, TitleLightDto book, TitleLightDto magazine) {
        super(id);
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

    public TitleLightDto getBook() {
        return book;
    }

    public void setBook(TitleLightDto book) {
        this.book = book;
    }

    public TitleLightDto getMagazine() {
        return magazine;
    }

    public void setMagazine(TitleLightDto magazine) {
        this.magazine = magazine;
    }

    public String getTitleName() { return (book == null ? magazine : book).getName(); }
}
