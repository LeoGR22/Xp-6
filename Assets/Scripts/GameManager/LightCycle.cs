using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightCycle : MonoBehaviour
{
    public Light2D luzAmbiente;
    public Light2D luzJanela;

    [Header("Modo de Teste (Hora Acelerada)")]
    public bool usarHorarioSimulado = false;
    [Range(0f, 24f)] public float horaSimulada = 6f;
    public float velocidadeSimulacao = 0.1f;
    public float suavidade = 2f;

    struct LightPoint
    {
        public float hora;
        public Color ambiente;
        public Color janela;

        public LightPoint(float h, string corAmb, string corJan)
        {
            hora = h;
            ambiente = HexToColor(corAmb);
            janela = HexToColor(corJan);
        }
    }

    List<LightPoint> pontosDeLuz;

    void Start()
    {
        pontosDeLuz = new List<LightPoint>()
        {
            new LightPoint(0f,  "483AA8", "000000"), 
            new LightPoint(6f,  "5460A6", "DDB51A"), 
            new LightPoint(12f, "B7B29E", "F3DB7D"), 
            new LightPoint(15f, "B071B7", "C8AA38"), 
            new LightPoint(18f, "6D60BF", "C8AA38"), 
        };

        if (!usarHorarioSimulado)
        {
            float horaAtual = System.DateTime.Now.Hour + System.DateTime.Now.Minute / 60f;
            AplicarCores(horaAtual, true);
        }
    }

    void Update()
    {
        if (usarHorarioSimulado)
        {
            horaSimulada += Time.deltaTime * velocidadeSimulacao;
            if (horaSimulada >= 24f) horaSimulada = 0f;

            AplicarCores(horaSimulada, false);
        }
    }

    void AplicarCores(float horaAtual, bool aplicarDireto)
    {
        LightPoint anterior = pontosDeLuz[0];
        LightPoint proximo = pontosDeLuz[0];

        for (int i = 0; i < pontosDeLuz.Count; i++)
        {
            LightPoint ponto = pontosDeLuz[i];
            if (ponto.hora > horaAtual)
            {
                proximo = ponto;
                anterior = i == 0 ? pontosDeLuz[pontosDeLuz.Count - 1] : pontosDeLuz[i - 1];
                break;
            }
        }

        float duracao = (proximo.hora - anterior.hora + 24f) % 24f;
        float tempoPassado = (horaAtual - anterior.hora + 24f) % 24f;
        float t = duracao == 0 ? 0 : tempoPassado / duracao;

        Color corAmb = Color.Lerp(anterior.ambiente, proximo.ambiente, t);
        Color corJan = Color.Lerp(anterior.janela, proximo.janela, t);

        if (aplicarDireto)
        {
            luzAmbiente.color = corAmb;
            luzJanela.color = corJan;
        }
        else
        {
            luzAmbiente.color = Color.Lerp(luzAmbiente.color, corAmb, Time.deltaTime * suavidade);
            luzJanela.color = Color.Lerp(luzJanela.color, corJan, Time.deltaTime * suavidade);
        }
    }

    static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color cor))
            return cor;
        return Color.magenta;
    }
}
