using UnityEngine;
using UnityEngine.SceneManagement;

public class Boton : MonoBehaviour

{
    public void IrJuego()
    {
        SceneManager.LoadScene("Juego");
    }
    public void IrCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
