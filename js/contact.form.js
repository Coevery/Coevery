  $(document).ready(function(){
        $('#send_message').click(function(e){
            
            //stop the form from being submitted
            e.preventDefault();
            
            /* declare the variables, var error is the variable that we use on the end
            to determine if there was an error or not */
            var error = false;
            var name = $('#name').val();
            var email = $('#email').val();
            var subject = $('#subject').val();
            var message = $('#message').val();
            
            if(name.length == 0){
                var error = true;
                $('#name_error').fadeIn(500);
            }else{
                $('#name_error').fadeOut(500);
            }
            if(email.length == 0 || email.indexOf('@') == '-1'){
                var error = true;
                $('#email_error').fadeIn(500);
            }else{
                $('#email_error').fadeOut(500);
            }
            if(subject.length == 0){
                var error = true;
                $('#subject_error').fadeIn(500);
            }else{
                $('#subject_error').fadeOut(500);
            }
            if(message.length == 0){
                var error = true;
                $('#message_error').fadeIn(500);
            }else{
                $('#message_error').fadeOut(500);
            }
            
            //now when the validation is done we check if the error variable is false (no errors)
            if(error == false){
                //disable the submit button to avoid spamming
                //and change the button text to Sending...
                $('#send_message').attr({'disabled' : 'true', 'value' : 'Sending...' });
                
                /* using the jquery's post(ajax) function and a lifesaver
                function serialize() which gets all the data from the form
                we submit it to send_email.php */
                $.post("send_email.php", $("#contact_form").serialize(),function(result){
                    //and after the ajax request ends we check the text returned
                    if(result == 'sent'){
                        //if the mail is sent remove the submit paragraph
                         $('#cf_submit_p').remove();
                        //and show the mail success div with fadeIn
                        $('#mail_success').fadeIn(500);
                    }else{
                        //show the mail failed div
                        $('#mail_fail').fadeIn(500);
                        //reenable the submit button by removing attribute disabled and change the text back to Send The Message
                        $('#send_message').removeAttr('disabled').attr('value', 'Send The Message');
                    }
                });
            }
        });    
    });
	  $(document).ready(function(){
        $('#order_message').click(function(e){
            
            //stop the form from being submitted
            e.preventDefault();
            
            /* declare the variables, var error is the variable that we use on the end
            to determine if there was an error or not */
            var error = false;
            var name = $('#order_name').val();
            var email = $('#order_email').val();
            var phone = $('#phone').val();
            var produk = $('#Product-type').val();
           
            if(name.length == 0){
                var error = true;
                $('#order_name_error').fadeIn(500);
            }else{
                $('#order_name_error').fadeOut(500);
            }
            if(email.length == 0 || email.indexOf('@') == '-1'){
                var error = true;
                $('#order_email_error').fadeIn(500);
            }else{
                $('#order_email_error').fadeOut(500);
            }
            if(phone.length == 0){
                var error = true;
                $('#phone_error').fadeIn(500);
            }else{
                $('#phone_error').fadeOut(500);
            }
            if(produk.length == 0){
                var error = true;
                $('#product_error').fadeIn(500);
            }else{
                $('#product_error').fadeOut(500);
            }
            
            //now when the validation is done we check if the error variable is false (no errors)
            if(error == false){
                //disable the submit button to avoid spamming
                //and change the button text to Sending...
                $('#order_message').attr({'disabled' : 'true', 'value' : 'Sending...' });
                
                /* using the jquery's post(ajax) function and a lifesaver
                function serialize() which gets all the data from the form
                we submit it to send_email.php */
                $.post("order_form.php", $("#order_form").serialize(),function(result){
                    //and after the ajax request ends we check the text returned
                    if(result == 'sent'){
                        //if the mail is sent remove the submit paragraph
                         $('#order_cf_submit_p').remove();
                        //and show the mail success div with fadeIn
                        $('#order_form_success').fadeIn(500);
                    }else{
                        //show the mail failed div
                        $('#order_form_fail').fadeIn(500);
                        //reenable the submit button by removing attribute disabled and change the text back to Send The Message
                        $('#order_message').removeAttr('disabled').attr('value', 'Order');
                    }
                });
            }
        });    
    });
		  $(document).ready(function(){
        $('#sign_message').click(function(e){
            
            //stop the form from being submitted
            e.preventDefault();
            
            /* declare the variables, var error is the variable that we use on the end
            to determine if there was an error or not */
            var error = false;
            var name = $('#sign_form_name').val();
            var email = $('#sign_form_email').val();
          
            if(name.length == 0){
                var error = true;
                $('#sign_name_error').fadeIn(500);
            }else{
                $('#sign_name_error').fadeOut(500);
            }
            if(email.length == 0 || email.indexOf('@') == '-1'){
                var error = true;
                $('#sign_email_error').fadeIn(500);
            }else{
                $('#sign_email_error').fadeOut(500);
            }
            
            
            //now when the validation is done we check if the error variable is false (no errors)
            if(error == false){
                //disable the submit button to avoid spamming
                //and change the button text to Sending...
                $('#sign_message').attr({'disabled' : 'true', 'value' : 'Sending...' });
                
                /* using the jquery's post(ajax) function and a lifesaver
                function serialize() which gets all the data from the form
                we submit it to send_email.php */
                $.post("sign_form.php", $("#sign_form").serialize(),function(result){
                    //and after the ajax request ends we check the text returned
                    if(result == 'sent'){
                        //if the mail is sent remove the submit paragraph
                         $('#sign_form_cf_submit_p').remove();
                        //and show the mail success div with fadeIn
                        $('#sign_form_mail_success').fadeIn(500);
                    }else{
                        //show the mail failed div
                        $('#sign_form_mail_fail').fadeIn(500);
                        //reenable the submit button by removing attribute disabled and change the text back to Send The Message
                        $('#sign_message').removeAttr('disabled').attr('value', 'Sign Up');
                    }
                });
            }
        });    
    });