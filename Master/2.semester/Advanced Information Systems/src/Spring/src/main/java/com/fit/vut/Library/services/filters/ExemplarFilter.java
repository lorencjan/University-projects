package com.fit.vut.Library.services.filters;

import java.util.Optional;

public class ExemplarFilter {

    protected String title;

    protected String state;

    protected Optional<Boolean> availability;


    public ExemplarFilter(String title, Optional<Boolean> availability){
        this.title = title;
        this.availability = availability;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getState() {
        return state;
    }

    public void setState(String state) {
        this.state = state;
    }

    public Optional<Boolean> getAvailability() {
        return availability;
    }

    public void setAvailability(Optional<Boolean> availability) {
        this.availability = availability;
    }
}
