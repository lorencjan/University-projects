package com.fit.vut.Library.application;

import javax.mail.*;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;
import java.util.Properties;
import java.util.logging.Level;
import java.util.logging.Logger;

public class EmailManager {

    private static final String account = "knihovna.pis@seznam.cz";
    private static final String password = "knihovna-PIS-2022";

    public static void sendMail(String recipient, String subject, String content) {
        Properties props = createProperties();
        Session session = createSession(props);

        Message msg = new MimeMessage(session);
        try {
            msg.setFrom(new InternetAddress(account));
            msg.setRecipient(Message.RecipientType.TO, new InternetAddress(recipient));
            msg.setSubject(subject);
            msg.setText(content);
            Transport.send(msg);
        } catch (MessagingException e) {
            Logger.getLogger(EmailManager.class.getName()).log(Level.SEVERE, "Failed to send e-mail!", e);
        }
    }

    private static Properties createProperties(){
        Properties props = new Properties();
        props.put("mail.smtp.host", "smtp.seznam.cz");
        props.put("mail.smtp.port", "587");
        props.put("mail.smtp.starttls.enable", "true");
        props.put("mail.smtp.auth", "true");

        return props;
    }

    private static Session createSession(Properties props){
        return Session.getInstance(props, new Authenticator() {
            @Override
            protected PasswordAuthentication getPasswordAuthentication() {
                return new PasswordAuthentication(account, password);
            }
        });
    }
}
