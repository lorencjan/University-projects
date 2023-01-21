package com.fit.vut.Library.services.filters;

import java.util.Date;

public class BorrowingFilter {

    private Date startedAfter;

    private Date endedAfter;

    private String state;

    public BorrowingFilter(Date startedAfter, Date endedAfter, String state){
        this.startedAfter = startedAfter;
        this.endedAfter = endedAfter;
        this.state = state;
    }


    public Date getStartedAfter() { return startedAfter; }

    public void setStartedAfter(Date startedAfter) { this.startedAfter = startedAfter; }

    public Date getEndedAfter() { return endedAfter; }

    public void setEndedAfter(Date endedAfter) { this.endedAfter = endedAfter; }

    public String getState() { return state; }

    public void setState(String state) { this.state = state; }
}
