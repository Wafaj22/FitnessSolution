using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessSolution.Controllers
{
    public class BlobsController : Controller
    {
  
        //make a container using code
        //1. prepare the container details with the information
        private CloudBlobContainer GetContainerInformation(string name)
        {
            //1.1 link with the appsettings.json to get the accesskey
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();
            CloudStorageAccount accountobject = CloudStorageAccount.Parse(configure["ConnectionStrings:BlobStorageConnection"]);

            //step2: add accesskey to account object by referring the appsetting.json            
            CloudBlobClient clientAgent = accountobject.CreateCloudBlobClient();

            //step3: give info about which container name actually you wish to refer/create
            CloudBlobContainer container = clientAgent.GetContainerReference(name);

            return container;
        }

        //2. this method used for creating the container
        public CloudBlobContainer CreateNewContainer(string name)
        {
            //2.1 get container information and its accesskey to do creation action.
            CloudBlobContainer container = GetContainerInformation(name);

            //2.2 create the container if the system found that the name does not exist in the list
            container.CreateIfNotExistsAsync();

            return container;
        }

        //3 learn to make simple upload blob function
        public bool SimpleUploadFile(string containerName, string blobName, string path, string extension)
        {
            //3.1 get container information and its accesskey to do the creation action.
            CloudBlobContainer container = CreateNewContainer(containerName);
            //3.2 assign a new name for your new blob item
            CloudBlockBlob blobitem = container.GetBlockBlobReference(blobName);

            blobitem.Properties.ContentType = extension;
            System.Diagnostics.Debug.WriteLine(extension);
            System.Diagnostics.Debug.WriteLine(path);

            //3.3 find the source image file and upload to the blob storage using the name that just created.
            using (var fileStream = System.IO.File.OpenRead(@path))
            {
                //await exercice.ExerciceImageFile.CopyToAsync(fileStream);
                blobitem.UploadFromStreamAsync(fileStream).Wait();
            }

            return true;
        }

        //5. Display all the images as a picture gallery (view)
        public string GetSingleBlob(string containerName, string blobName)
        {
            CloudBlobContainer container = CreateNewContainer(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            return blob.Uri.ToString();
        }

        //5. Display all the images as a picture gallery (view)
        public ActionResult PictureGallery(string name, string message = null)
        {
            ViewBag.msg = message;
            CloudBlobContainer container = GetContainerInformation(name);

            //create a new empty list to contain the blobs information
            List<string> blobitems = new List<string>();

            //read the blob storage items using below code
            BlobResultSegment result = container.ListBlobsSegmentedAsync(null).Result;

            //split one by one items from the list
            foreach (IListBlobItem item in result.Results)
            {
                //blob type = block blob/ append blob/ directory
                if (item.GetType() == typeof(CloudBlockBlob)) //filter the blob type
                {
                    CloudBlockBlob singleblob = (CloudBlockBlob)item;
                    //block blob = video / audio / images (jpg /png/ gif)
                    if (Path.GetExtension(singleblob.Name.ToString()) == ".jpg")
                    {
                        //add item info to list<string>
                        blobitems.Add(singleblob.Name + "#" + singleblob.Uri.ToString());
                    }
                }
            }
            return View(blobitems);
        }

        public ActionResult DeleteBlob(string imagename, string name)
        {
            CloudBlobContainer container = GetContainerInformation(name);
            string blobname; 
            string message;

            try
            {
                //find the item based on name inside the blob storage
                CloudBlockBlob blobitem = container.GetBlockBlobReference(imagename);
                blobname = blobitem.Name;

                //delete the item once you found it
                blobitem.DeleteIfExistsAsync();
                message = blobname + " is successfully deleted from the blob storage";
            }
            catch (Exception ex)
            {
                message = "Technical issue: " + ex.ToString() + ". Please try to delete the file again.";
            }
            return RedirectToAction("picturegallery", "Blobs", new { message });
        }
        public ActionResult Download(string imagename, string imagelink, string name)
        {
            CloudBlobContainer container = GetContainerInformation(name);
            string message;

            //to do downloading
            try
            {
                CloudBlockBlob item = container.GetBlockBlobReference(imagename);
                //create destination filepath inside your pc
                var outputitem = System.IO.File.OpenWrite(@"C:\\Users\\Dhushyen\\Desktop\\" + imagename);
                item.DownloadToStreamAsync(outputitem).Wait();
                message = imagename + " is successfully downloaded from " + imagelink + " to your desktop! Please check it!";
                outputitem.Close();
            }
            catch (Exception ex)
            {
                message = "Unable to download the file of " + imagename + "\\nTechnical issue: " + ex.ToString() + ". Please try downloading the file again!";
            }
            return RedirectToAction("picturegallery", "Blobs", new { message });
        }
    }
}
